using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace RaidCrawler.Core.Structures;

/// <summary>
/// See also https://github.com/kwsch/PKHeX/blob/master/PKHeX.Core/Saves/Substructures/Gen9/RaidSpawnList9.cs
/// </summary>
public class Raid(Span<byte> Data, TeraRaidMapParent MapParent = TeraRaidMapParent.Paldea)
{
    public const byte SIZE = 0x20;
    private readonly byte[] Data = Data.ToArray(); // Raw data

    public readonly TeraRaidMapParent MapParent = MapParent;

    public bool IsValid      => Validate();
    public bool IsActive     => ReadUInt32LittleEndian(Data.AsSpan(0x00)) == 1;
    public uint Area         => ReadUInt32LittleEndian(Data.AsSpan(0x04));
    public uint LotteryGroup => ReadUInt32LittleEndian(Data.AsSpan(0x08));
    public uint Den          => ReadUInt32LittleEndian(Data.AsSpan(0x0C));
    public uint Seed         => ReadUInt32LittleEndian(Data.AsSpan(0x10));
    public uint Flags        => ReadUInt32LittleEndian(Data.AsSpan(0x18));
    public bool IsBlack      => Flags == 1;
    public bool IsEvent      => Flags >= 2;

    public int TeraType      => GetTeraType(Seed);
    public uint Difficulty   => GetDifficulty(Seed);

    public uint EC           => GenericRaidData[0];
    public uint PID          => GenericRaidData[2];
    public bool IsShiny      => GenericRaidData[3] == 1;

    private uint[] GenericRaidData => GenerateGenericRaidData(Seed);

    /// <summary>
/// Gets the raw 32-byte raid data.
/// </summary>
/// <returns>The raid's raw data buffer (32 bytes).</returns>
public byte[] GetData()  => Data;

    /// <summary>
    /// Checks whether the raid record is valid (Seed is nonzero, the raid is active, and the area is valid for the configured map parent) and, when valid, initializes the generated raid fields.
    /// </summary>
    /// <returns>`true` if the raid is valid and generated raid data was initialized; `false` otherwise.</returns>
    private bool Validate()
    {
        if (Seed == 0 || !IsActive)
            return false;
        if (!IsValidMap())
            return false;

        GenerateGenericRaidData(Seed);
        return true;
    }

    /// <summary>
    /// Determines whether the raid's Area value is valid for the current MapParent.
    /// </summary>
    /// <returns>`true` if Area is within the valid range for the MapParent (Paldea: ≤22, Kitakami: ≤11, Blueberry: ≤8), `false` otherwise.</returns>
    private bool IsValidMap()
    {
        return MapParent switch
        {
            TeraRaidMapParent.Paldea    => Area <= 22,
            TeraRaidMapParent.Kitakami  => Area <= 11,
            TeraRaidMapParent.Blueberry => Area <= 8,
            _                           => false
        };
    }

    /// <summary>
    /// Determines the Tera type index generated from the provided seed.
    /// </summary>
    /// <param name="Seed">Seed value used to initialize the deterministic RNG.</param>
    /// <returns>An integer in the range 0–17 representing the selected Tera type.</returns>
    private static int GetTeraType(uint Seed)
    {
        var rng = new Xoroshiro128Plus(Seed);
        return (int)rng.NextInt(18);
    }

    /// <summary>
    /// Generates the raid's generic values (EC, TID/SID, PID, and a shiny indicator) derived from the provided seed.
    /// </summary>
    /// <returns>A uint[4] containing EC, TIDSID, PID, and the Shiny flag (1 if shiny, 0 otherwise), in that order.</returns>
    private static uint[] GenerateGenericRaidData(uint Seed)
    {
        var rng = new Xoroshiro128Plus(Seed);
        uint EC = (uint)rng.NextInt();
        uint TIDSID = (uint)rng.NextInt();
        uint PID = (uint)rng.NextInt();
        bool IsShiny = ((PID >> 16) ^ (PID & 0xFFFF)) >> 4 == ((TIDSID >> 16) ^ (TIDSID & 0xFFFF)) >> 4;
        var Shiny = IsShiny ? 1u : 0;
        return [EC, TIDSID, PID, Shiny];
    }

    /// <summary>
    /// Gets a raid difficulty value derived from the provided seed.
    /// </summary>
    /// <param name="Seed">Seed used to initialize the Xoroshiro128Plus pseudo-random generator.</param>
    /// <returns>Difficulty value between 0 and 99 inclusive.</returns>
    private static uint GetDifficulty(uint Seed)
    {
        var rng = new Xoroshiro128Plus(Seed);
        return (uint)rng.NextInt(100);
    }

    /// <summary>
    /// Attempts to populate the provided PK9 with encounter data using the given shiny criterion and seed, and retries with a fallback shiny criterion based on the PK9's current shiny status if the first attempt fails.
    /// </summary>
    /// <param name="pk">The PK9 instance to populate with generated encounter data.</param>
    /// <param name="param">Generation parameters that influence encounter data creation.</param>
    /// <param name="isShiny">The desired shiny criterion for the initial generation attempt.</param>
    /// <param name="seed">The RNG seed used for encounter data generation.</param>
    public void GenerateDataPK9(PK9 pk, GenerateParam9 param, Shiny isShiny, uint seed)
    {
        var criteria = new EncounterCriteria { Shiny = isShiny };
        bool check = Encounter9RNG.GenerateData(pk, param, criteria, seed);
        if (!check)
        {
            criteria = new EncounterCriteria { Shiny = pk.IsShiny ? Shiny.Always : Shiny.Random };
            Encounter9RNG.GenerateData(pk, param, criteria, seed);
        }
    }
}

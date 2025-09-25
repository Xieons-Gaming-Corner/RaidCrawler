namespace RaidCrawler.WinForms.SubForms;

public partial class ItemIDs : Form
{
    /// <summary>
    /// Initializes the ItemIDs form, sets checkboxes corresponding to any recognized item IDs in the provided list, and assigns images for the capsule, cap, and patch picture boxes.
    /// </summary>
    /// <param name="IDs">List of item ID integers; IDs 645, 795, 1606, and 1904–1908 will set their corresponding checkboxes (Ability Capsule, Bottle Cap, Ability Patch, Sweet, Salty, Sour, Bitter, Spicy).</param>
    public ItemIDs(List<int> IDs)
    {
        InitializeComponent();

        foreach (int ID in IDs)
        {
            switch (ID)
            {
                case 645:
                    CheckAbilityCapsule.Checked = true;
                    break;
                case 795:
                    CheckBottleCap.Checked = true;
                    break;
                case 1606:
                    CheckAbilityPatch.Checked = true;
                    break;
                case 1904:
                    CheckSweet.Checked = true;
                    break;
                case 1905:
                    CheckSalty.Checked = true;
                    break;
                case 1906:
                    CheckSour.Checked = true;
                    break;
                case 1907:
                    CheckBitter.Checked = true;
                    break;
                case 1908:
                    CheckSpicy.Checked = true;
                    break;
            }
        }
        PicCapsule.Image = (Image?)
            PKHeX.Drawing.PokeSprite.Properties.Resources.ResourceManager.GetObject(
                "aitem_645"
            );
        PicCap.Image = (Image?)
            PKHeX.Drawing.PokeSprite.Properties.Resources.ResourceManager.GetObject(
                "aitem_795"
            );
        PicPatch.Image = (Image?)
            PKHeX.Drawing.PokeSprite.Properties.Resources.ResourceManager.GetObject(
                "aitem_1606"
            );
    }
}

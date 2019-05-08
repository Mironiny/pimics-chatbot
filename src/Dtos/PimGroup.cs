namespace PimBot.Dto
{
    public class PimGroup
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Description_2 { get; set; }

        public string System_Status { get; set; }

        public string LocalDescription { get; set; }

        public string LocalDescription2 { get; set; }

        public string Base_Unit { get; set; }

        public string Picture_Document_ID { get; set; }

        public string Template_Code { get; set; }

        public string Publication_Group { get; set; }

        public string Short_Text { get; set; }

        public string Created_By { get; set; }

        public string Updated_By { get; set; }

        public string Additional_Information_1 { get; set; }

        public string Additional_Information_2 { get; set; }

        public string Additional_Information_3 { get; set; }

        public string Additional_Information_4 { get; set; }

        public string Additional_Information_5 { get; set; }

        public string ETag { get; set; }

        protected bool Equals(PimGroup other)
        {
            return string.Equals(Code, other.Code) && string.Equals(Description, other.Description) && string.Equals(Description_2, other.Description_2) && string.Equals(System_Status, other.System_Status) && string.Equals(LocalDescription, other.LocalDescription) && string.Equals(LocalDescription2, other.LocalDescription2) && string.Equals(Base_Unit, other.Base_Unit) && string.Equals(Picture_Document_ID, other.Picture_Document_ID) && string.Equals(Template_Code, other.Template_Code) && string.Equals(Publication_Group, other.Publication_Group) && string.Equals(Short_Text, other.Short_Text) && string.Equals(Created_By, other.Created_By) && string.Equals(Updated_By, other.Updated_By) && string.Equals(Additional_Information_1, other.Additional_Information_1) && string.Equals(Additional_Information_2, other.Additional_Information_2) && string.Equals(Additional_Information_3, other.Additional_Information_3) && string.Equals(Additional_Information_4, other.Additional_Information_4) && string.Equals(Additional_Information_5, other.Additional_Information_5) && string.Equals(ETag, other.ETag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PimGroup) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description_2 != null ? Description_2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (System_Status != null ? System_Status.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LocalDescription != null ? LocalDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LocalDescription2 != null ? LocalDescription2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Base_Unit != null ? Base_Unit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Picture_Document_ID != null ? Picture_Document_ID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Template_Code != null ? Template_Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Publication_Group != null ? Publication_Group.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Short_Text != null ? Short_Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Created_By != null ? Created_By.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Updated_By != null ? Updated_By.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Additional_Information_1 != null ? Additional_Information_1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Additional_Information_2 != null ? Additional_Information_2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Additional_Information_3 != null ? Additional_Information_3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Additional_Information_4 != null ? Additional_Information_4.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Additional_Information_5 != null ? Additional_Information_5.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ETag != null ? ETag.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

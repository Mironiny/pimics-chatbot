// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

namespace PimBot.Dto
{
    /// <summary>
    /// PimFeature dto class.
    /// </summary>
    public class PimFeature
    {
        public string Source { get; set; }

        public string Code { get; set; }

        public string Group_System_Number { get; set; }

        public string Source_Type { get; set; }

        public string Source_Code { get; set; }

        public int Line_No { get; set; }

        public string Line_Type { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public string Unit_ID { get; set; }

        public int Values { get; set; }

        public int Possible_values { get; set; }

        public string Field_Type { get; set; }

        public string Usage_Type_Code { get; set; }

        public string Feature_Type { get; set; }

        public bool Not_inheritance_value { get; set; }

        public string Composed_value { get; set; }

        public string Connection_Characteristics { get; set; }

        public string Formatting { get; set; }

        public bool Required { get; set; }

        public string Search_Feature_ID { get; set; }

        public string Feature_ID_Reference { get; set; }

        public string Document_ID { get; set; }

        public string Field_Format { get; set; }

        public string Alignment { get; set; }

        public string Output_Format_Features { get; set; }

        public string Print { get; set; }

        public string Web { get; set; }

        public bool Description_Texts { get; set; }

        public string Unit_Shorthand_Description { get; set; }

        public string Value_ID { get; set; }

        public string Value_ID_Reference { get; set; }

        public int Order { get; set; }

        public string ETag { get; set; }
    }
}

namespace Phone_api.Common
{
    // <summary>
    /// Lớp chứa các hằng số liên quan đến SQL (stored procedure và tham số)
    /// </summary>
    public static class SqlConstants
    {
        public const string AddPhoneProcedure = "AddPhone";
        public const string ParamModel = "@Model";
        public const string ParamPrice = "@Price";
        public const string ParamStock = "@Stock";
        public const string ParamBrandId = "@BrandId";
        public const string ParamResult = "@Result";
    }
}

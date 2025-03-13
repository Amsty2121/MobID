namespace SmartPass.Repository.Models.Enums
{
    public enum AccessLevel
    {
        None,
        //BlockedAccess - no access for all, only for God
        Blocked = -1,
        //Guest - access only at guest area 
        Guest,
        //Simple Employee - access only at unprotected employee area
        Employee,
        //Special Employee - acces to all employ area and to a special zone
        SpecialZoneEmployee,
        //Technical Employee (Cleanning, Electicity ...) - Access to all zones and to room with stuff
        TechnicalEmployee,
        //Can access all zones
        Administrator,
        God
    }
}

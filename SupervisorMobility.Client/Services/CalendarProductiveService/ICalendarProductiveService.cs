namespace SupervisorMobility.Client.Services.CalendarProductiveService
{
    public interface ICalendarProductiveService
    {

        Task<List<Holiday>> GetHolidaysInService(int year);
     
        //guarda dias en server
        bool AddHolidayToService(Holiday ToAdd);
      



        Task<List<Holiday>> GetHolidaysInYear(int year);
        Task<List<Holiday>> UpdateHolidaysInYear( int year, List<Holiday>? holiday = null);

        //Task DeleteHoliday(int id);

    }
}

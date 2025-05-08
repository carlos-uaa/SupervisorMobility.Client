namespace SupervisorMobility.Client.Services.CalendarProductiveService
{
    public interface ICalendarProductiveService
    {

        List<Holiday> GetHolidaysInService();
        bool AddHolidaysInService(List<Holiday> toAdd);
        bool AddHolidayToService(Holiday ToAdd);
        bool UpdateHolidayInService(Holiday toUpdate);



        //Task<List<Holiday>> GetHolidays();

        //Task<Holiday> GetHolidayById(int id);

        //Task<Holiday> CreateHoliday(Holiday holiday);
        //Task<Holiday> CreateMultipleHolidays(List<Holiday> holidays);

        //Task<bool> UpdateHoliday(Holiday holiday);
        //Task<bool> UpdateMultipleHolidays(List<Holiday> holiday);

        //Task DeleteHoliday(int id);

    }
}

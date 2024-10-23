namespace SupervisorMobility.Client.Data.Entities.PaginationEntities
{
    public class JOPaginationDto
    {
        public int Total { get; set; }
        public List<JobObservationNulls> JobObservations { get; set; }
        public JOCountPaginationDto CountPagination { get; set; }
    }

}

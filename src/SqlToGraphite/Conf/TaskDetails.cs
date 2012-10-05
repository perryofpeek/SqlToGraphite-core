namespace SqlToGraphite.Conf
{
    public class TaskDetails
    {
        public TaskDetails(string role, int frequency, string jobName)
        {
            this.Role = role;
            this.Frequency = frequency;
            this.JobName = jobName;
        }

        public string Role { get; set; }

        public int Frequency { get; set; }

        public string JobName { get; set; }
    }
}
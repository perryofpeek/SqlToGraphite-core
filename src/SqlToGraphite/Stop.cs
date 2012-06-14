namespace SqlToGraphite
{
    public class Stop : IStop
    {
        private bool status;

        public void SetStop(bool value)
        {
            status = value;
        }

        public bool ShouldStop()
        {
            return status;
        }
    }
}
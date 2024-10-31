using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation
{
    public class DiskInformation : Information
    {
        private List<string> _disksName = new();
        private List<long> _diskTotalSize = new();
        private List<long> _disksFreeSize = new();
        private int _numberDisk;

        public List<string> getDiskName()
        {
            return _disksName;
        }

        public List<long> getDiskTotalSize()
        {
            return _diskTotalSize;
        }

        public List<long> getDisksFreeSizes()
        {
            return _disksFreeSize;
        }

        public void setNumberDisk(int numberDisk)
        {
            _numberDisk = numberDisk;
        }

        public override string ToString()
        {
            return string.Join("Nom disque : ", _disksName) + "\n" + string.Join("Total size : ", _diskTotalSize) + "\n" + string.Join("Free size : ", _disksFreeSize) + "\n" +
                   _numberDisk;
        }

    }
}

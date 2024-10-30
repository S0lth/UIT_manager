using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation
{
    public class DiskInformation : Information
    {
        private List<string> _disksName = new();
        private List<long> _diskTotalSize = new();
        private List<long> _disksFreeSize = new();
        private int _numberDisk;

        public void addDiskName(string diskName)
        {
            _disksName.Add(diskName);
        }

        public void addDiskTotalSize(long diskTotalSize)
        {
            _diskTotalSize.Add(diskTotalSize);
        }

        public void addDisksFreeSizesk(long disksFreeSize)
        {
            _disksFreeSize.Add(disksFreeSize);
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

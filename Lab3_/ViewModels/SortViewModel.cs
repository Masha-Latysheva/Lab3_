namespace Lab3_.ViewModels
{
    public enum SortState
    {
        No,
        WeightAsc,
        WeightDesc,
        VolumeAsc,
        VolumeDesc
    }

    public class SortViewModel
    {
        public SortViewModel(SortState sortOrder)
        {
            WeightSort = sortOrder == SortState.WeightAsc ? SortState.WeightDesc : SortState.WeightAsc;
            VolumeSort = sortOrder == SortState.VolumeAsc ? SortState.VolumeDesc : SortState.VolumeAsc;
            CurrentState = sortOrder;
        }

        public SortState WeightSort { get; set; }
        public SortState VolumeSort { get; set; }
        public SortState CurrentState { get; set; }
    }
}
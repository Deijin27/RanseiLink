namespace Core.Models.Interfaces
{
    public interface IMoveRange : IDataWrapper
    {
        bool GetInRange(int position);

        void SetInRange(int position, bool isInRange);
    }
}
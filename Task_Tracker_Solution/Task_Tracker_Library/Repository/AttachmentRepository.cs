using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class AttachmentRepository : TTBaseRepository<TaskAttachment>
    {
        public AttachmentRepository(TTDBContext _context) : base(_context)
        {
        }        
    }
}

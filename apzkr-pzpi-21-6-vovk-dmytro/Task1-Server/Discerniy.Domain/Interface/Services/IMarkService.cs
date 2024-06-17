using Discerniy.Domain.Requests;
using Discerniy.Domain.Responses;

namespace Discerniy.Domain.Interface.Services
{
    public interface IMarkService
    {
        public Task<IEnumerable<MarkResponse>> GetMarksFromGroup(string groupId);
        public Task<MarkResponse> GetMark(string markId);
        public Task<MarkResponse> CreateMark(CreateMarkRequest request);
        public Task<MarkResponse> UpdateMark(string markId, UpdateMarkRequest request);
        public Task<MarkResponse> DeleteMark(string markId);
    }
}

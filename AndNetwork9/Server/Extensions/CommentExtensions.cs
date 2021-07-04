using System;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Extensions
{
    public static class CommentExtensions
    {
        public static async Task<Comment> AddCommentToDataContext(this Comment comment, ClanDataContext data,
            Member author)
        {
            comment = comment with
            {
                Author = author,
                AuthorId = author.Id,
                CreateTime = DateTime.UtcNow,
                Children = Array.Empty<Comment>(),
                Id = 0,
                LastEditTime = null,
            };
            if (comment.ParentId is not null)
            {
                comment.Parent = await data.Comments.FindAsync(comment.ParentId);
                if (comment.Parent is null) throw new ArgumentException();
            }
            else
            {
                comment.Parent = null;
            }

            EntityEntry<Comment> result = await data.Comments.AddAsync(comment);
            return result.Entity;
        }
    }
}
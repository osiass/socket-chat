using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Shared
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public User Follower { get; set; } = null!;
        public int FollowingId { get; set; }
        public User Following { get; set; } = null!;
        
    }
}

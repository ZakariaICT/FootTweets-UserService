using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Repositories

{
    public class RemoveDuplicateUsers : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);
        private readonly UserRepo _userRepo;

        public RemoveDuplicateUsers(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _userRepo.RemoveDuplicateUsers();

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }

}

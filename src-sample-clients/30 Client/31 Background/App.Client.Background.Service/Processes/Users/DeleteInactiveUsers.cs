using App.Domain.DataRepositoryContracts;
using App.Domain.Processes.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes;
using Signals.Core.Processes.Recurring;
using Signals.Core.Processing.Results;

namespace App.Client.Background.Service.Processes.Users
{
    public class DeleteInactiveUsers : NoOverlapRecurringProcess<VoidResult>
    {
        public override RecurrencePatternConfiguration Profile => new DailyRecurrencePatternConfiguration(1) { RunNow = true }.At(2, 0, 0);

        [Import] private IUserRepository UserRepository { get; set; }

        /// <summary>
        /// Sync process
        /// </summary>
        /// <returns></returns>
        public override VoidResult Sync()
        {
            var inactiveUsers = UserRepository.GetAllInactive();

            foreach (var user in inactiveUsers)
            {
                var result = Continue<UserProcesses.DeleteUser>().With(new DeleteUserRequestDto
                {
                    Id = user.Id
                });

                if (result.IsFaulted) return Fail(result);
            }

            return Ok();
        }
    }
}

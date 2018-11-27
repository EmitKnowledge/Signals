using App.Service.DataRepository.Base;
using App.Service.DataRepositoryContracts;
using App.Service.DomainEntities.Users;
using Dapper;
using NodaTime;
using Signals.Aspects.DI.Attributes;

namespace App.Service.DataRepository.Users
{
    [Export(typeof(IUserImageRepository))]
    internal class UserImageRepository : BaseDbRepository<UserImage>, IUserImageRepository
    {
        /// <summary>
        /// Return the 40px image
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        public UserImage GetImage(int userId, UserImageDimensionsEnum imageSize)
        {
            return Using(connection =>
            {
                UserImage userImage = null;

                switch (imageSize)
                {
                    case UserImageDimensionsEnum.ImageOriginal:
                        userImage = base.FirstOrDefault(image => new UserImage
                        {
                            Id = image.Id,
                            UserId = image.UserId,
                            CreatedOn = image.CreatedOn,
                            ImageOriginal = image.ImageOriginal
                        }, x => x.UserId == userId);
                        break;

                    case UserImageDimensionsEnum.ImageSizeVarationA:
                        userImage = base.FirstOrDefault(image => new UserImage
                        {
                            Id = image.Id,
                            UserId = image.UserId,
                            CreatedOn = image.CreatedOn,
                            ImageSizeVarationA = image.ImageSizeVarationA
                        }, x => x.UserId == userId);
                        break;

                    case UserImageDimensionsEnum.ImageSizeVarationB:
                        userImage = base.FirstOrDefault(image => new UserImage
                        {
                            Id = image.Id,
                            UserId = image.UserId,
                            CreatedOn = image.CreatedOn,
                            ImageSizeVarationB = image.ImageSizeVarationB
                        }, x => x.UserId == userId);
                        break;

                    case UserImageDimensionsEnum.ImageSizeVarationC:
                        userImage = base.FirstOrDefault(image => new UserImage
                        {
                            Id = image.Id,
                            UserId = image.UserId,
                            CreatedOn = image.CreatedOn,
                            ImageSizeVarationC = image.ImageSizeVarationC
                        }, x => x.UserId == userId);
                        break;
                }

                if (userImage == null)
                {
                    userImage = new UserImage
                    {
                        Id = int.MaxValue,
                        UserId = userId,
                        CreatedOn = SystemClock.Instance.GetCurrentInstant()
                    };
                };

                return userImage;
            });
        }

        /// <summary>
        /// Update user image
        /// </summary>
        /// <param name="profileImage"></param>
        public void UpdateImage(UserImage profileImage)
        {
            Using(connection =>
            {
                connection.Execute(@"UPDATE [USERIMAGE]
                                     SET
                                     ImageOriginal = @ImageOriginal,
                                     ImageSizeVarationA = @ImageSizeVarationA,
                                     ImageSizeVarationB = @ImageSizeVarationB,
                                     ImageSizeVarationC = @ImageSizeVarationC
                                     WHERE UserId = @UserId", new
                {
                    profileImage.ImageOriginal,
                    profileImage.ImageSizeVarationA,
                    profileImage.ImageSizeVarationB,
                    profileImage.ImageSizeVarationC,
                    profileImage.UserId
                });
            });
        }
    }
}
using App.Domain.Entities.Users;

namespace App.Domain.DataRepositoryContracts
{
    public interface IUserImageRepository : IRepository<UserImage>
    {
        /// <summary>
        /// Return the 40px image
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        UserImage GetImage(int userId, UserImageDimensionsEnum imageSize);

        /// <summary>
        /// Update profile image
        /// </summary>
        /// <param name="userImage"></param>
        void UpdateImage(UserImage userImage);
    }
}
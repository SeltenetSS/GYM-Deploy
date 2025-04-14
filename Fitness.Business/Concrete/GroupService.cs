﻿using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class GroupService : IGroupService
    {
        private readonly IGroupDal _groupDal;
        private readonly IUserDal _userDal;
        private readonly IGroupUserDal _groupUserDal;

        public GroupService(IGroupDal groupDal, IUserDal userDal, IGroupUserDal groupUserDal)
        {
            _groupDal = groupDal;
            _userDal = userDal;
            _groupUserDal = groupUserDal;
        }

        public async Task<Group> CreateGroupAsync(GroupCreateDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                PackageId = dto.PackageId
            };

            await _groupDal.Add(group);
            return group;
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            return await _groupDal.GetByIdAsync(id);
        }

        public async Task<Group> UpdateGroupAsync(GroupUpdateDto dto)
        {
            var group = await _groupDal.GetByIdAsync(dto.Id);
            if (group == null) return null;

            group.Name = dto.Name;
            group.PackageId = dto.PackageId;

            await _groupDal.Update(group);
            return group;
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _groupDal.GetByIdAsync(id);
            if (group == null) return false;

            await _groupDal.Delete(group);
            return true;
        }

        public async Task<bool> AddUserToGroupAsync(AddUserToGroupDto dto)
        {
            var group = await _groupDal.GetByIdAsync(dto.GroupId);
            var user = await _userDal.GetByIdAsync(dto.UserId);

            if (group == null || user == null)
                return false;

            var alreadyExists = await _groupUserDal.AnyAsync(gu => gu.GroupId == dto.GroupId && gu.UserId == dto.UserId);
            if (alreadyExists)
                return false;

            var groupUser = new GroupUser
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId
            };

            await _groupUserDal.Add(groupUser);
            return true;
        }
    }

}

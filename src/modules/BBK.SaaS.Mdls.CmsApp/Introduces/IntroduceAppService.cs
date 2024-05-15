using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.Introduces
{
    public class IntroduceAppService : SaaSAppServiceBase, IIntroduceAppService
    {
        private readonly IRepository<Introduce, long> _introduceRepository;
        private readonly IRepository<User, long> _Userrepository;
        public IntroduceAppService(IRepository<Introduce, long> introduceRepository, IRepository<User, long> Userrepository)
        {
            _introduceRepository = introduceRepository;
            _Userrepository = Userrepository;
        }


        public async Task<PagedResultDto<IntroduceEditDto>> GetAll(IntroduceSearch input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                var query = _introduceRepository
                          .GetAll().AsNoTracking().OrderByDescending(x => x.CreationTime).Include(x => x.Article).Where(x => x.Article != null)
                          .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.FullName.ToLower().Contains(input.Search.ToLower()) || x.Article.Title.ToLower().Contains(input.Search.ToLower()))
                          .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value).ToList()
                          .Select(x => new IntroduceEditDto
                          {
                              Id = x.Id,
                              Description = x.Description,
                              FullName = x.FullName,
                              Email = x.Email,
                              Phone = x.Phone,
                              Status = x.Status,
                              CreationTime = x.CreationTime,
                              Article = x.Article,
                              ArticleId = x.ArticleId,
                              CreatorUserId = x.CreatorUserId,
                          }).ToList();

                //var result = (from q in query
                //              join us in _Userrepository.GetAll() on q.CreatorUserId equals us.Id
                //              select new IntroduceEditDto
                //              {
                //                  Id = q.Id,
                //                  Description = q.Description,
                //                  FullName = q.FullName,
                //                  Email = q.Email,
                //                  Phone =q.Phone,
                //                  Status = q.Status,
                //                  CreationTime = q.CreationTime,
                //                  Article = q.Article,
                //                  ArticleId = q.ArticleId,
                //                  CreatorUserId = q.CreatorUserId,
                //                  Name = us.Name
                //              }).ToList();


                var Count = query.Count();
                return new PagedResultDto<IntroduceEditDto>(
                    Count,
					query.ToList()
                    );


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }
        public async Task<long> Create(IntroduceEditDto input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                Introduce Introduce = new Introduce();
                Introduce.ArticleId = input.ArticleId;
                Introduce.FullName = input.FullName;
                Introduce.Email = input.Email;
                Introduce.Phone = input.Phone;
                Introduce.Description = input.Description;
                Introduce.Status = 1;
                var newId = await _introduceRepository.InsertAndGetIdAsync(Introduce);
                return newId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<long> Update(IntroduceEditDto input)
        {
            try
            {
                Introduce Introduce = _introduceRepository.FirstOrDefault(x => x.Id == input.Id);
                Introduce.ArticleId = input.ArticleId;
                Introduce.FullName = input.FullName;
                Introduce.Email = input.Email;
                Introduce.Phone = input.Phone;
                Introduce.Description = input.Description;
                Introduce.Status = input.Status;
                await _introduceRepository.UpdateAsync(Introduce);
                return input.Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public async Task Delete(long? Id)
        {
            try
            {
                if (Id.HasValue)
                {
                    var intro = _introduceRepository.Get(Id.Value);
                    intro.IsDeleted = true;
                    intro.DeletionTime = DateTime.Now;
                    await _introduceRepository.UpdateAsync(intro);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<int> GetCountByUserId()
        {
            var query = (await _introduceRepository.GetAllListAsync()).Where(x => x.CreatorUserId == AbpSession.UserId && x.CreationTime.Day == DateTime.Now.Day && x.CreationTime.Month == DateTime.Now.Month && x.CreationTime.Year == DateTime.Now.Year);
            int count = query.Count();
            return count;
        }

        public async Task<PagedResultDto<IntroduceEditDto>> GetAllByUserType(IntroduceSearch input)
        {
            try
            {
                if (AbpSession.TenantId.HasValue == false)
                {
                    var unit = UnitOfWorkManager.Current;
                    unit.SetTenantId(1);
                }
                var query = _introduceRepository
                          .GetAll().AsNoTracking()
                          .OrderByDescending(x => x.CreationTime)
                          .Include(x => x.Article)
                          .Where(x => x.Article != null && x.CreatorUserId == AbpSession.UserId)
                          .WhereIf(!string.IsNullOrEmpty(input.Search), x => x.FullName.ToLower().Contains(input.Search.ToLower()) || x.Article.Title.ToLower().Contains(input.Search.ToLower()))
                          .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value).ToList()
                          .Select(x => new IntroduceEditDto
                          {
                              Id = x.Id,
                              Description = x.Description,
                              FullName = x.FullName,
                              Email = x.Email,  
                              Phone = x.Phone,
                              Status = x.Status,
                              CreationTime = x.CreationTime,
                              Article = x.Article,
                              ArticleId = x.ArticleId,
                              CreatorUserId = x.CreatorUserId,
                          }).ToList();

                var result = (from q in query
                              join us in _Userrepository.GetAll() on q.CreatorUserId equals us.Id
                              select new IntroduceEditDto
                              {
                                  Id = q.Id,
                                  Description = q.Description,
                                  FullName = q.FullName,
                                  Email = q.Email,
                                  Phone = q.Phone,
                                  Status = q.Status,
                                  CreationTime = q.CreationTime,
                                  Article = q.Article,
                                  ArticleId = q.ArticleId,
                                  CreatorUserId = q.CreatorUserId,
                                  Name = us.Name
                              }).ToList();


                var Count = result.Count();
                return new PagedResultDto<IntroduceEditDto>(
                    Count,
                    result.ToList()
                    );


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }

        public async Task<int> GetCountByUserIdForMobile()
        {
            var query = _introduceRepository.GetAll()
                .AsNoTracking()
                .Where(x => x.CreatorUserId == AbpSession.UserId && x.CreationTime.Day == DateTime.Now.Day && x.CreationTime.Month == DateTime.Now.Month && x.CreationTime.Year == DateTime.Now.Year);
            int count = query.Count();
            return count;
        }
    }
}

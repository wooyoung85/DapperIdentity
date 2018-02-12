﻿using System;
using DapperIdentity.Data.Identity;
using IdentityTest.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace DapperIdentity.Web
{
    // 이 응용 프로그램에서 사용되는 응용 프로그램 사용자 관리자를 구성합니다. UserManager는 ASP.NET Identity에서 정의하며 응용 프로그램에서 사용됩니다.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>() as DbManager));
            // 사용자 이름에 대한 유효성 검사 논리 구성
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 암호에 대한 유효성 검사 논리 구성
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 사용자 잠금 기본값 구성
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 2단계 인증 공급자를 등록합니다. 이 응용 프로그램은 사용자 확인 코드를 받는 단계에서 전화 및 전자 메일을 사용합니다.
            // 공급자 및 플러그 인을 여기에 쓸 수 있습니다.
            manager.RegisterTwoFactorProvider("전화 코드", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "보안 코드는 {0}입니다."
            });
            manager.RegisterTwoFactorProvider("전자 메일 코드", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "보안 코드",
                BodyFormat = "보안 코드는 {0}입니다."
            });
            
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}

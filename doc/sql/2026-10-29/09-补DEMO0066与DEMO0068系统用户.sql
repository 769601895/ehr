-- 审批找人：员工.post_id → 角色绑定岗 → 员工工号 → SysUser.JobNum
-- DEMO0068(人事专员) 上级角色「人力资源部经理」绑定岗 000006，对应员工 DEMO0066，
-- 但 SysUser 原先无 JobNum=DEMO0066，导致 ResolveUsersForRole 空人 → NoAssigneeOnPath。
-- 密码与 DEMO0004 相同（测试账号）。

SET NOCOUNT ON;

DECLARE @pwd nvarchar(1024);
DECLARE @org bigint, @pos bigint, @tenant bigint, @atype int, @sex int, @age int, @card int, @cult int, @ord int;
SELECT
    @pwd = Password, @org = OrgId, @pos = PosId, @tenant = TenantId, @atype = AccountType,
    @sex = Sex, @age = Age, @card = CardType, @cult = CultureLevel, @ord = OrderNo
FROM SysUser WHERE Account = N'DEMO0004';

IF @pwd IS NULL
BEGIN
    RAISERROR(N'缺少模板账号 DEMO0004，请先准备后再执行', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM SysUser WHERE JobNum = N'DEMO0066' OR Account = N'DEMO0066')
INSERT INTO SysUser (
    Id, Account, Password, RealName, Sex, Age, Birthday, CardType, CultureLevel,
    OrderNo, Status, AccountType, OrgId, PosId, TokenVersion, JobNum, NickName,
    TenantId, CreateTime, CreateUserId, CreateUserName, IsDelete
) VALUES (
    864460000000066, N'DEMO0066', @pwd, N'demo4-人力资源部经理', @sex, @age, '2000-01-01', @card, @cult,
    @ord, 1, @atype, @org, @pos, 1, N'DEMO0066', N'DEMO0066',
    @tenant, GETDATE(), 1300000000101, N'超级管理员', 0
);

IF NOT EXISTS (SELECT 1 FROM SysUser WHERE JobNum = N'DEMO0068' OR Account = N'DEMO0068')
INSERT INTO SysUser (
    Id, Account, Password, RealName, Sex, Age, Birthday, CardType, CultureLevel,
    OrderNo, Status, AccountType, OrgId, PosId, TokenVersion, JobNum, NickName,
    TenantId, CreateTime, CreateUserId, CreateUserName, IsDelete
) VALUES (
    864460000000068, N'DEMO0068', @pwd, N'demo6-人事专员', @sex, @age, '2000-01-01', @card, @cult,
    @ord, 1, @atype, @org, @pos, 1, N'DEMO0068', N'DEMO0068',
    @tenant, GETDATE(), 1300000000101, N'超级管理员', 0
);

DECLARE @src bigint = 858497544667205; -- DEMO0004
DECLARE @mgr bigint = (SELECT Id FROM SysUser WHERE JobNum = N'DEMO0066');
DECLARE @staff bigint = (SELECT Id FROM SysUser WHERE JobNum = N'DEMO0068');

IF @mgr IS NOT NULL AND NOT EXISTS (SELECT 1 FROM SysUserRole WHERE UserId = @mgr)
INSERT INTO SysUserRole (Id, UserId, RoleId)
SELECT 864460100000000 + ROW_NUMBER() OVER (ORDER BY RoleId), @mgr, RoleId
FROM SysUserRole WHERE UserId = @src;

IF @staff IS NOT NULL AND NOT EXISTS (SELECT 1 FROM SysUserRole WHERE UserId = @staff)
INSERT INTO SysUserRole (Id, UserId, RoleId)
SELECT 864460200000000 + ROW_NUMBER() OVER (ORDER BY RoleId), @staff, RoleId
FROM SysUserRole WHERE UserId = @src;

SELECT Id, Account, RealName, JobNum, Status FROM SysUser WHERE JobNum IN (N'DEMO0066', N'DEMO0068');

-- 人员补充信息：家庭成员、教育经历、工作履历、职称情况
-- 每人多行，emp_id 关联 h_emp_mstr.emp_id，sort_no 为显示顺序

IF OBJECT_ID(N'dbo.h_emp_family', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_emp_family (
        id            BIGINT        NOT NULL CONSTRAINT PK_h_emp_family PRIMARY KEY,
        emp_id        NVARCHAR(50)  NOT NULL,
        sort_no       INT           NOT NULL CONSTRAINT DF_h_emp_family_sort DEFAULT (0),
        relation      NVARCHAR(20)  NULL,
        member_name   NVARCHAR(50)  NULL,
        workplace     NVARCHAR(100) NULL,
        duty          NVARCHAR(50)  NULL
    );
    CREATE INDEX IX_h_emp_family_emp_id ON dbo.h_emp_family (emp_id, sort_no, id);
END
GO

IF OBJECT_ID(N'dbo.h_emp_edu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_emp_edu (
        id          BIGINT        NOT NULL CONSTRAINT PK_h_emp_edu PRIMARY KEY,
        emp_id      NVARCHAR(50)  NOT NULL,
        sort_no     INT           NOT NULL CONSTRAINT DF_h_emp_edu_sort DEFAULT (0),
        start_date  DATETIME      NULL,
        end_date    DATETIME      NULL,
        school      NVARCHAR(100) NULL,
        major       NVARCHAR(100) NULL,
        degree      NVARCHAR(20)  NULL,
        is_highest  BIT           NOT NULL CONSTRAINT DF_h_emp_edu_highest DEFAULT (0),
        remark      NVARCHAR(500) NULL
    );
    CREATE INDEX IX_h_emp_edu_emp_id ON dbo.h_emp_edu (emp_id, sort_no, id);
END
GO

IF OBJECT_ID(N'dbo.h_emp_career', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_emp_career (
        id          BIGINT        NOT NULL CONSTRAINT PK_h_emp_career PRIMARY KEY,
        emp_id      NVARCHAR(50)  NOT NULL,
        sort_no     INT           NOT NULL CONSTRAINT DF_h_emp_career_sort DEFAULT (0),
        start_date  DATETIME      NULL,
        end_date    DATETIME      NULL,
        workplace   NVARCHAR(100) NULL,
        dept_name   NVARCHAR(100) NULL,
        post_name   NVARCHAR(50)  NULL,
        remark      NVARCHAR(500) NULL
    );
    CREATE INDEX IX_h_emp_career_emp_id ON dbo.h_emp_career (emp_id, sort_no, id);
END
GO

IF OBJECT_ID(N'dbo.h_emp_title', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.h_emp_title (
        id           BIGINT        NOT NULL CONSTRAINT PK_h_emp_title PRIMARY KEY,
        emp_id       NVARCHAR(50)  NOT NULL,
        sort_no      INT           NOT NULL CONSTRAINT DF_h_emp_title_sort DEFAULT (0),
        title_name   NVARCHAR(50)  NULL,
        title_level  NVARCHAR(20)  NULL,
        cert_no      NVARCHAR(50)  NULL,
        obtain_date  DATETIME      NULL,
        remark       NVARCHAR(500) NULL
    );
    CREATE INDEX IX_h_emp_title_emp_id ON dbo.h_emp_title (emp_id, sort_no, id);
END
GO

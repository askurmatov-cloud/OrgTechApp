-- Схема базы данных user102_db (объекты _05_02_*)
-- Сгенерировано из работающей базы данных.
USE [user102_db];
GO

CREATE TABLE [dbo].[_05_02_Orgtehnika] (
    [KodOrgtehniki] int IDENTITY(1,1) NOT NULL,
    [InventarnyyNomer] nvarchar(30) NOT NULL,
    [Naimenovanie] nvarchar(100) NOT NULL,
    [Model] nvarchar(100) NOT NULL,
    [SeriynyyNomer] nvarchar(50) NOT NULL,
    [KodPostavshchika] int NOT NULL,
    [DataPostupleniya] date NOT NULL,
    [SrokGarantiiMesyatsev] int NOT NULL,
    [Stoimost] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT (N'IN_USE'),
    [Kommentariy] nvarchar(500) NULL,
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    [FotoPut] nvarchar(260) NULL,
    CONSTRAINT [PK__05_02_Orgtehnika] PRIMARY KEY ([KodOrgtehniki]),
    CONSTRAINT [UQ__05_02_Orgtehnika_InventarnyyNomer] UNIQUE ([InventarnyyNomer]),
    CONSTRAINT [UQ__05_02_Orgtehnika_SeriynyyNomer] UNIQUE ([SeriynyyNomer]),
    CONSTRAINT [CK__05_02_Orgtehnika_Garantiya] CHECK ([SrokGarantiiMesyatsev]>=(0)),
    CONSTRAINT [CK__05_02_Orgtehnika_Status] CHECK ([Status]=N'WRITTEN_OFF' OR [Status]=N'IN_REPAIR' OR [Status]=N'IN_USE'),
    CONSTRAINT [CK__05_02_Orgtehnika_Stoimost] CHECK ([Stoimost]>=(0))
);
GO

CREATE TABLE [dbo].[_05_02_Peremeshchenie] (
    [KodPeremeshcheniya] int IDENTITY(1,1) NOT NULL,
    [KodOrgtehniki] int NOT NULL,
    [KodOperatsii] char(3) NOT NULL,
    [DataOperatsii] date NOT NULL,
    [KodIspolnitelya] int NOT NULL,
    [KodPoluchatelya] int NULL,
    [NomerDokumenta] nvarchar(50) NULL,
    [Prichina] nvarchar(250) NULL,
    [Kommentariy] nvarchar(500) NULL,
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Peremeshchenie] PRIMARY KEY ([KodPeremeshcheniya]),
    CONSTRAINT [CK__05_02_Peremeshchenie_Operatsiya] CHECK ([KodOperatsii]='REC' AND [KodPoluchatelya] IS NULL AND [Prichina] IS NULL OR [KodOperatsii]='TRN' AND [KodPoluchatelya] IS NOT NULL OR [KodOperatsii]='WRT' AND [KodPoluchatelya] IS NULL AND [Prichina] IS NOT NULL OR ([KodOperatsii]='INV' OR [KodOperatsii]='REP') AND [KodPoluchatelya] IS NULL)
);
GO

CREATE TABLE [dbo].[_05_02_Podrazdelenie] (
    [KodPodrazdeleniya] int IDENTITY(1,1) NOT NULL,
    [Naimenovanie] nvarchar(100) NOT NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Podrazdelenie] PRIMARY KEY ([KodPodrazdeleniya]),
    CONSTRAINT [UQ__05_02_Podrazdelenie_Naimenovanie] UNIQUE ([Naimenovanie])
);
GO

CREATE TABLE [dbo].[_05_02_Polzovatel] (
    [KodPolzovatelya] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(50) NOT NULL,
    [HeshParolya] nvarchar(128) NOT NULL,
    [Soly] nvarchar(32) NOT NULL,
    [KodRoli] nvarchar(20) NOT NULL,
    [ElektronnayaPochta] nvarchar(100) NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataPoslednegoVhoda] datetime2(0) NULL,
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Polzovatel] PRIMARY KEY ([KodPolzovatelya]),
    CONSTRAINT [UQ__05_02_Polzovatel_Login] UNIQUE ([Login])
);
GO

CREATE TABLE [dbo].[_05_02_Postavshchik] (
    [KodPostavshchika] int IDENTITY(1,1) NOT NULL,
    [Naimenovanie] nvarchar(150) NOT NULL,
    [YuridicheskiyAdres] nvarchar(250) NOT NULL,
    [Telefon] nvarchar(30) NULL,
    [ElektronnayaPochta] nvarchar(100) NULL,
    [FIODirektora] nvarchar(150) NULL,
    [NomerScheta] nvarchar(20) NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Postavshchik] PRIMARY KEY ([KodPostavshchika])
);
GO

CREATE TABLE [dbo].[_05_02_Rol] (
    [KodRoli] nvarchar(20) NOT NULL,
    [Naimenovanie] nvarchar(100) NOT NULL,
    [Opisanie] nvarchar(250) NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Rol] PRIMARY KEY ([KodRoli]),
    CONSTRAINT [UQ__05_02_Rol_Naimenovanie] UNIQUE ([Naimenovanie])
);
GO

CREATE TABLE [dbo].[_05_02_Sotrudnik] (
    [KodSotrudnika] int IDENTITY(1,1) NOT NULL,
    [Familiya] nvarchar(50) NOT NULL,
    [Imya] nvarchar(50) NOT NULL,
    [Otchestvo] nvarchar(50) NULL,
    [Dolzhnost] nvarchar(100) NOT NULL,
    [KodPodrazdeleniya] int NOT NULL,
    [Telefon] nvarchar(30) NULL,
    [ElektronnayaPochta] nvarchar(100) NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataIzmeneniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Sotrudnik] PRIMARY KEY ([KodSotrudnika])
);
GO

CREATE TABLE [dbo].[_05_02_TipOperatsii] (
    [KodOperatsii] char(3) NOT NULL,
    [Naimenovanie] nvarchar(50) NOT NULL,
    [Aktivno] bit NOT NULL DEFAULT ((1)),
    CONSTRAINT [PK__05_02_TipOperatsii] PRIMARY KEY ([KodOperatsii]),
    CONSTRAINT [UQ__05_02_TipOperatsii_Naimenovanie] UNIQUE ([Naimenovanie])
);
GO

CREATE TABLE [dbo].[_05_02_Zayavka] (
    [KodZayavki] int IDENTITY(1,1) NOT NULL,
    [KodOrgtehniki] int NOT NULL,
    [KodPolzovatelya] int NOT NULL,
    [TipZayavki] nvarchar(30) NOT NULL,
    [Opisanie] nvarchar(500) NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT (N'Новая'),
    [DataSozdaniya] datetime2(0) NOT NULL DEFAULT (sysdatetime()),
    [DataObrabotki] datetime2(0) NULL,
    [KodIspolnitelya] int NULL,
    [KommentariyAdministratora] nvarchar(500) NULL,
    [Versiya] timestamp NOT NULL,
    CONSTRAINT [PK__05_02_Zayavka] PRIMARY KEY ([KodZayavki]),
    CONSTRAINT [CK__05_02_Zayavka_Status] CHECK ([Status]=N'Отклонена' OR [Status]=N'Выполнена' OR [Status]=N'В работе' OR [Status]=N'Новая'),
    CONSTRAINT [CK__05_02_Zayavka_Tip] CHECK ([TipZayavki]=N'Другое' OR [TipZayavki]=N'Консультация' OR [TipZayavki]=N'Замена' OR [TipZayavki]=N'Обслуживание' OR [TipZayavki]=N'Ремонт')
);
GO

-- Внешние ключи

ALTER TABLE [dbo].[_05_02_Orgtehnika] ADD CONSTRAINT [FK__05_02_Orgtehnika_Postavshchik] FOREIGN KEY ([KodPostavshchika]) REFERENCES [dbo].[_05_02_Postavshchik] ([KodPostavshchika]);
GO

ALTER TABLE [dbo].[_05_02_Peremeshchenie] ADD CONSTRAINT [FK__05_02_Peremeshchenie_Operatsiya] FOREIGN KEY ([KodOperatsii]) REFERENCES [dbo].[_05_02_TipOperatsii] ([KodOperatsii]);
GO

ALTER TABLE [dbo].[_05_02_Peremeshchenie] ADD CONSTRAINT [FK__05_02_Peremeshchenie_Orgtehnika] FOREIGN KEY ([KodOrgtehniki]) REFERENCES [dbo].[_05_02_Orgtehnika] ([KodOrgtehniki]);
GO

ALTER TABLE [dbo].[_05_02_Peremeshchenie] ADD CONSTRAINT [FK__05_02_Peremeshchenie_Ispolnitel] FOREIGN KEY ([KodIspolnitelya]) REFERENCES [dbo].[_05_02_Polzovatel] ([KodPolzovatelya]);
GO

ALTER TABLE [dbo].[_05_02_Peremeshchenie] ADD CONSTRAINT [FK__05_02_Peremeshchenie_Poluchatel] FOREIGN KEY ([KodPoluchatelya]) REFERENCES [dbo].[_05_02_Sotrudnik] ([KodSotrudnika]);
GO

ALTER TABLE [dbo].[_05_02_Polzovatel] ADD CONSTRAINT [FK__05_02_Polzovatel_Rol] FOREIGN KEY ([KodRoli]) REFERENCES [dbo].[_05_02_Rol] ([KodRoli]);
GO

ALTER TABLE [dbo].[_05_02_Sotrudnik] ADD CONSTRAINT [FK__05_02_Sotrudnik_Podrazdelenie] FOREIGN KEY ([KodPodrazdeleniya]) REFERENCES [dbo].[_05_02_Podrazdelenie] ([KodPodrazdeleniya]);
GO

ALTER TABLE [dbo].[_05_02_Zayavka] ADD CONSTRAINT [FK__05_02_Zayavka_Ispolnitel] FOREIGN KEY ([KodIspolnitelya]) REFERENCES [dbo].[_05_02_Polzovatel] ([KodPolzovatelya]);
GO

ALTER TABLE [dbo].[_05_02_Zayavka] ADD CONSTRAINT [FK__05_02_Zayavka_Orgtehnika] FOREIGN KEY ([KodOrgtehniki]) REFERENCES [dbo].[_05_02_Orgtehnika] ([KodOrgtehniki]);
GO

ALTER TABLE [dbo].[_05_02_Zayavka] ADD CONSTRAINT [FK__05_02_Zayavka_Avtor] FOREIGN KEY ([KodPolzovatelya]) REFERENCES [dbo].[_05_02_Polzovatel] ([KodPolzovatelya]);
GO

-- Представления

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Представление: активные пользователи для авторизации */
CREATE   VIEW dbo._05_02_vw_AktivnyePolzovateli AS
SELECT
    p.KodPolzovatelya,
    p.Login,
    p.HeshParolya,
    p.Soly,
    p.KodRoli,
    r.Naimenovanie AS Rol,
    p.ElektronnayaPochta,
    p.DataPoslednegoVhoda
FROM dbo._05_02_Polzovatel AS p
JOIN dbo._05_02_Rol AS r
  ON r.KodRoli = p.KodRoli
WHERE p.Aktivno = 1
  AND r.Aktivno = 1;

GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Представление: полная история операций */
CREATE   VIEW dbo._05_02_vw_IstoriyaPeremeshcheniy AS
SELECT
    p.KodPeremeshcheniya,
    p.KodOrgtehniki,
    o.InventarnyyNomer,
    o.Naimenovanie,
    o.Model,
    o.SeriynyyNomer,
    p.KodOperatsii,
    t.Naimenovanie AS Operatsiya,
    p.DataOperatsii,
    z.Login AS Ispolnitel,
    s.KodSotrudnika,
    NULLIF(LTRIM(RTRIM(CONCAT(s.Familiya, N' ', s.Imya, N' ', s.Otchestvo))), N'') AS Poluchatel,
    d.Naimenovanie AS Podrazdelenie,
    p.NomerDokumenta,
    p.Prichina,
    p.Kommentariy,
    p.DataSozdaniya
FROM dbo._05_02_Peremeshchenie AS p
JOIN dbo._05_02_Orgtehnika AS o
  ON o.KodOrgtehniki = p.KodOrgtehniki
JOIN dbo._05_02_TipOperatsii AS t
  ON t.KodOperatsii = p.KodOperatsii
JOIN dbo._05_02_Polzovatel AS z
  ON z.KodPolzovatelya = p.KodIspolnitelya
LEFT JOIN dbo._05_02_Sotrudnik AS s
  ON s.KodSotrudnika = p.KodPoluchatelya
LEFT JOIN dbo._05_02_Podrazdelenie AS d
  ON d.KodPodrazdeleniya = s.KodPodrazdeleniya;

GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Представление: наличие по подразделениям (для отчёта) */
CREATE   VIEW dbo._05_02_vw_NalichiePoPodrazdeleniyam AS
SELECT
    d.KodPodrazdeleniya,
    d.Naimenovanie AS Podrazdelenie,
    COUNT(v.KodOrgtehniki) AS Kolichestvo
FROM dbo._05_02_Podrazdelenie AS d
LEFT JOIN dbo._05_02_vw_OrgtehnikaSostoyanie AS v
       ON v.KodPodrazdeleniya = d.KodPodrazdeleniya
      AND v.Status <> N'WRITTEN_OFF'
WHERE d.Aktivno = 1
GROUP BY d.KodPodrazdeleniya, d.Naimenovanie;

GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE   VIEW dbo._05_02_vw_OrgtehnikaSostoyanie AS
SELECT
    o.KodOrgtehniki,
    o.InventarnyyNomer,
    o.Naimenovanie,
    o.Model,
    o.SeriynyyNomer,
    o.Status,
    CASE o.Status
        WHEN N'IN_USE' THEN N'V ekspluatatsii'
        WHEN N'IN_REPAIR' THEN N'V remonte'
        WHEN N'WRITTEN_OFF' THEN N'Spisana'
    END AS StatusNaimenovanie,
    o.DataPostupleniya,
    o.SrokGarantiiMesyatsev,
    o.Stoimost,
    o.Kommentariy,
    o.FotoPut,
    s.KodSotrudnika,
    NULLIF(LTRIM(RTRIM(CONCAT(s.Familiya, N' ', s.Imya, N' ', s.Otchestvo))), N'') AS FIO,
    d.KodPodrazdeleniya,
    d.Naimenovanie AS Podrazdelenie,
    p.DataOperatsii AS DataPeredachi,
    p.NomerDokumenta AS NomerDokumentaPeredachi
FROM dbo._05_02_Orgtehnika AS o
OUTER APPLY
(
    SELECT TOP (1) x.DataOperatsii, x.NomerDokumenta
    FROM dbo._05_02_Peremeshchenie AS x
    WHERE x.KodOrgtehniki = o.KodOrgtehniki
      AND x.KodOperatsii = 'TRN'
    ORDER BY x.DataOperatsii DESC, x.KodPeremeshcheniya DESC
) AS p
LEFT JOIN dbo._05_02_Sotrudnik AS s
       ON s.KodSotrudnika =
       (
           SELECT TOP (1) y.KodPoluchatelya
           FROM dbo._05_02_Peremeshchenie AS y
           WHERE y.KodOrgtehniki = o.KodOrgtehniki
             AND y.KodOperatsii = 'TRN'
           ORDER BY y.DataOperatsii DESC, y.KodPeremeshcheniya DESC
       )
LEFT JOIN dbo._05_02_Podrazdelenie AS d
       ON d.KodPodrazdeleniya = s.KodPodrazdeleniya;

GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE VIEW dbo._05_02_vw_Zayavki AS
SELECT
    z.KodZayavki,
    z.KodOrgtehniki,
    o.InventarnyyNomer,
    o.Naimenovanie,
    o.Model,
    z.KodPolzovatelya,
    av.Login AS AvtorLogin,
    z.TipZayavki,
    z.TipZayavki AS TipNaimenovanie,
    z.Opisanie,
    z.Status,
    z.Status AS StatusNaimenovanie,
    z.DataSozdaniya,
    z.DataObrabotki,
    z.KodIspolnitelya,
    isp.Login AS IspolnitelLogin,
    z.KommentariyAdministratora,
    z.Versiya
FROM dbo._05_02_Zayavka AS z
JOIN dbo._05_02_Orgtehnika AS o ON o.KodOrgtehniki = z.KodOrgtehniki
JOIN dbo._05_02_Polzovatel AS av ON av.KodPolzovatelya = z.KodPolzovatelya
LEFT JOIN dbo._05_02_Polzovatel AS isp ON isp.KodPolzovatelya = z.KodIspolnitelya;

GO

-- Хранимые процедуры

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Оформление операции: поступление, передача, списание, ремонт, инвентаризация */
CREATE   PROCEDURE dbo._05_02_sp_OformitOperatsiyu
    @KodOrgtehniki INT,
    @KodOperatsii CHAR(3),
    @KodIspolnitelya INT,
    @KodPoluchatelya INT = NULL,
    @DataOperatsii DATE = NULL,
    @NomerDokumenta NVARCHAR(50) = NULL,
    @Prichina NVARCHAR(250) = NULL,
    @Kommentariy NVARCHAR(500) = NULL,
    @KodPeremeshcheniya INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @DataOperatsii IS NULL
        SET @DataOperatsii = CAST(GETDATE() AS DATE);

    IF @KodOperatsii IS NULL
       OR @KodOperatsii NOT IN ('REC', 'TRN', 'WRT', 'REP', 'INV')
        THROW 51001, N'Nedopustimyy tip operatsii.', 1;

    IF NOT EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Orgtehnika
           WHERE KodOrgtehniki = @KodOrgtehniki
       )
        THROW 51002, N'Orgtehnika ne naydena.', 1;

    IF NOT EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Polzovatel
           WHERE KodPolzovatelya = @KodIspolnitelya
             AND Aktivno = 1
       )
        THROW 51003, N'Ispolnitel ne nayden ili otklyuchen.', 1;

    IF @KodOperatsii = 'TRN'
       AND NOT EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Sotrudnik
           WHERE KodSotrudnika = @KodPoluchatelya
             AND Aktivno = 1
       )
        THROW 51004, N'Poluchatel ne nayden ili otklyuchen.', 1;

    IF @KodOperatsii = 'REC' AND @KodPoluchatelya IS NOT NULL
        THROW 51005, N'Dlya postupleniya poluchatel ne ukazyvaetsya.', 1;
    IF @KodOperatsii = 'REC' AND @Prichina IS NOT NULL
        THROW 51006, N'Dlya postupleniya prichina ne ukazyvaetsya.', 1;
    IF @KodOperatsii = 'TRN' AND @KodPoluchatelya IS NULL
        THROW 51007, N'Dlya peredachi neobhodim poluchatel.', 1;
    IF @KodOperatsii = 'WRT'
       AND (@KodPoluchatelya IS NOT NULL OR @Prichina IS NULL)
        THROW 51008, N'Dlya spisaniya neobhodima prichina i ne ukazyvaetsya poluchatel.', 1;
    IF @KodOperatsii IN ('REP', 'INV') AND @KodPoluchatelya IS NOT NULL
        THROW 51009, N'Dlya etoy operatsii poluchatel ne ukazyvaetsya.', 1;

    IF @DataOperatsii > CAST(GETDATE() AS DATE)
        THROW 51010, N'Data operatsii ne mozhet byt v budushchem.', 1;

    IF @DataOperatsii <
       (
           SELECT DataPostupleniya
           FROM dbo._05_02_Orgtehnika
           WHERE KodOrgtehniki = @KodOrgtehniki
       )
        THROW 51011, N'Data operatsii ranshe daty postupleniya.', 1;

    IF EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Orgtehnika
           WHERE KodOrgtehniki = @KodOrgtehniki
             AND Status = N'WRITTEN_OFF'
       )
        THROW 51012, N'Orgtehnika uzhe spisana.', 1;

    IF @KodOperatsii = 'REC'
       AND EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Peremeshchenie
           WHERE KodOrgtehniki = @KodOrgtehniki
       )
        THROW 51013, N'Dlya dannoy orgtehniki uzhe est istoriya operatsiy.', 1;

    IF EXISTS
       (
           SELECT 1
           FROM dbo._05_02_Peremeshchenie
           WHERE KodOrgtehniki = @KodOrgtehniki
             AND DataOperatsii > @DataOperatsii
       )
        THROW 51014, N'Data operatsii ranshe posledney operatsii.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo._05_02_Peremeshchenie
            (KodOrgtehniki, KodOperatsii, DataOperatsii, KodIspolnitelya,
             KodPoluchatelya, NomerDokumenta, Prichina, Kommentariy)
        VALUES
            (@KodOrgtehniki, @KodOperatsii, @DataOperatsii, @KodIspolnitelya,
             @KodPoluchatelya, @NomerDokumenta, @Prichina, @Kommentariy);

        SET @KodPeremeshcheniya = CAST(SCOPE_IDENTITY() AS INT);

        UPDATE dbo._05_02_Orgtehnika
        SET Status =
            CASE @KodOperatsii
                WHEN 'REC' THEN N'IN_USE'
                WHEN 'TRN' THEN N'IN_USE'
                WHEN 'WRT' THEN N'WRITTEN_OFF'
                WHEN 'REP' THEN N'IN_REPAIR'
                ELSE Status
            END,
            DataIzmeneniya = SYSDATETIME()
        WHERE KodOrgtehniki = @KodOrgtehniki;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Поиск оргтехники для формы/API */
CREATE   PROCEDURE dbo._05_02_sp_PoiskOrgtehniki
    @Poisk NVARCHAR(100) = NULL,
    @KodPodrazdeleniya INT = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.KodOrgtehniki,
        v.InventarnyyNomer,
        v.Naimenovanie,
        v.Model,
        v.SeriynyyNomer,
        v.Status,
        v.StatusNaimenovanie,
        v.DataPostupleniya,
        v.Stoimost,
        v.KodSotrudnika,
        v.FIO,
        v.KodPodrazdeleniya,
        v.Podrazdelenie
    FROM dbo._05_02_vw_OrgtehnikaSostoyanie AS v
    WHERE
        (
            @Poisk IS NULL
            OR v.Naimenovanie LIKE N'%' + @Poisk + N'%'
            OR v.Model LIKE N'%' + @Poisk + N'%'
            OR v.SeriynyyNomer LIKE N'%' + @Poisk + N'%'
            OR v.InventarnyyNomer LIKE N'%' + @Poisk + N'%'
        )
        AND (@KodPodrazdeleniya IS NULL OR v.KodPodrazdeleniya = @KodPodrazdeleniya)
        AND (@Status IS NULL OR v.Status = @Status)
    ORDER BY v.Naimenovanie, v.Model;
END;

GO


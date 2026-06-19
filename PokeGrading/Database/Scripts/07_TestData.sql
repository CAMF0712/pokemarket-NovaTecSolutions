USE PokeGrading;
GO

/* =========================================================
ADMIN DE PRUEBA
Password: Admin123
========================================================= */

IF NOT EXISTS (
SELECT 1
FROM USERS
WHERE email = 'admin@poketest.com'
)
BEGIN

INSERT INTO USERS
(
    user_id,
    role_id,
    email,
    alias,
    password_hash,
    country,
    preferred_language,
    status,
    active,
    created_at
)
VALUES
(
    NEWID(),
    3,
    'admin@poketest.com',
    'MainAdmin',
    '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
    'CR',
    'ES',
    'ACTIVE',
    1,
    GETUTCDATE()
);

END;
GO

/* =========================================================
USUARIO NORMAL DE PRUEBA
Password: User123
========================================================= */

IF NOT EXISTS (
SELECT 1
FROM USERS
WHERE email = 'user@poketest.com'
)
BEGIN

INSERT INTO USERS
(
    user_id,
    role_id,
    email,
    alias,
    password_hash,
    country,
    preferred_language,
    status,
    active,
    created_at
)
VALUES
(
    NEWID(),
    1,
    'user@poketest.com',
    'TestUser',
    '$2a$10$EixZaYVK1fsbw1ZfbX3OXePaWxn96p36WQoeG6Lruj3vjPGga31lm',
    'CR',
    'ES',
    'ACTIVE',
    1,
    GETUTCDATE()
);

END;
GO

/* =========================================================
OBTENER IDS
========================================================= */

DECLARE @AdminId UNIQUEIDENTIFIER =
(
SELECT TOP 1 user_id
FROM USERS
WHERE email = 'admin@poketest.com'
);

IF @AdminId IS NULL
BEGIN
THROW 50001,
'No se encontro admin@poketest.com',
1;
END;

DECLARE @UserId UNIQUEIDENTIFIER =
(
SELECT TOP 1 user_id
FROM USERS
WHERE email = 'user@poketest.com'
);

IF @UserId IS NULL
BEGIN
THROW 50002,
'No se encontro user@poketest.com',
1;
END;
GO

/* =========================================================
CLIENTE B2B DE PRUEBA
========================================================= */

IF NOT EXISTS
(
SELECT 1
FROM B2B_CLIENTS
WHERE company_name = 'Pokemon Store CR'
)
BEGIN

INSERT INTO B2B_CLIENTS
(
    client_id,
    company_name,
    legal_name,
    commercial_contact,
    technical_contact,
    billing_address,
    plan_name,
    monthly_quota,
    sla,
    active,
    created_at
)
VALUES
(
    NEWID(),
    'Pokemon Store CR',
    'Pokemon Store Costa Rica',
    'ventas@pokemonstore.cr',
    'soporte@pokemonstore.cr',
    'San Jose, Costa Rica',
    'ENTERPRISE',
    1000,
    '24x7',
    1,
    GETUTCDATE()
);

END;
GO

/* =========================================================
SUSCRIPCION DE PRUEBA
========================================================= */

DECLARE @SubscriptionUser UNIQUEIDENTIFIER =
(
SELECT TOP 1 user_id
FROM USERS
WHERE email='user@poketest.com'
);

IF NOT EXISTS
(
SELECT 1
FROM SUBSCRIPTIONS
WHERE user_id = @SubscriptionUser
)
BEGIN

INSERT INTO SUBSCRIPTIONS
(
    subscription_id,
    user_id,
    plan_name,
    start_date,
    end_date,
    active
)
VALUES
(
    NEWID(),
    @SubscriptionUser,
    'FREE',
    GETUTCDATE(),
    DATEADD(YEAR,1,GETUTCDATE()),
    1
);

END;
GO

/* =========================================================
AUDITORIA DE PRUEBA
========================================================= */

DECLARE @AuditUser UNIQUEIDENTIFIER =
(
SELECT TOP 1 user_id
FROM USERS
WHERE email='admin@poketest.com'
);

INSERT INTO AUDIT_LOGS
(
audit_id,
user_id,
action_type,
entity_name,
entity_id,
timestamp
)
VALUES
(
NEWID(),
@AuditUser,
'CREATE',
'SYSTEM_SEED',
NULL,
GETUTCDATE()
);
GO

INSERT INTO ALGORITHM_VERSIONS
(
    version_id,
    version_name,
    description,
    created_at,
    active
)
VALUES
(
    NEWID(),
    'v1.0',
    'Initial grading algorithm',
    GETUTCDATE(),
    1
);

DECLARE @ClientId UNIQUEIDENTIFIER =
(
    SELECT TOP 1 client_id
    FROM B2B_CLIENTS
);

INSERT INTO API_KEYS
(
    api_key_id,
    client_id,
    label,
    api_key_hash,
    status,
    created_at
)
VALUES
(
    NEWID(),
    @ClientId,
    'Test Key',
    '25001f3135f6ab613043e8d6812a53fe', --MiApiKey123
    'ACTIVE',
    GETUTCDATE()
);
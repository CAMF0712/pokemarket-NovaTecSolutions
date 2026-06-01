USE PokeGrading;
GO

IF NOT EXISTS (SELECT 1 FROM USERS WHERE email = 'admin@poketest.com')
BEGIN
    INSERT INTO USERS(
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
    VALUES(
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

DECLARE @UserId UNIQUEIDENTIFIER =
(
    SELECT TOP 1 user_id
    FROM USERS
    WHERE email='admin@poketest.com'
);

IF @UserId IS NULL
BEGIN
    THROW 50001, 'No se encontro el usuario admin@poketest.com para TestData.', 1;
END;

INSERT INTO B2B_CLIENTS(
    client_id,
    company_name,
    legal_name,
    monthly_quota,
    active,
    created_at
)
VALUES(
    NEWID(),
    'Pokemon Store CR',
    'Pokemon Store Costa Rica',
    1000,
    1,
    GETUTCDATE()
);

INSERT INTO SUBSCRIPTIONS(
    subscription_id,
    user_id,
    plan_name,
    start_date,
    end_date,
    active
)
VALUES(
    NEWID(),
    @UserId,
    'FREE',
    GETUTCDATE(),
    DATEADD(YEAR,1,GETUTCDATE()),
    1
);

INSERT INTO AUDIT_LOGS(
    audit_id,
    user_id,
    action_type,
    entity_name,
    timestamp
)
VALUES(
    NEWID(),
    @UserId,
    'CREATE',
    'USER',
    GETUTCDATE()
);
GO
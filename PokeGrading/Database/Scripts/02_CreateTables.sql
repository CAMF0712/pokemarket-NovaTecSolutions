USE PokeGrading;
GO

CREATE TABLE ROLES (
    role_id INT NOT NULL PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);

CREATE TABLE USERS (
    user_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    role_id INT NOT NULL,
    email VARCHAR(255) NOT NULL,
    alias VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    country VARCHAR(50),
    preferred_language VARCHAR(20),
    status VARCHAR(50),
    active BIT NOT NULL,
    created_at DATETIME2 NOT NULL,
    last_login DATETIME2
);

CREATE TABLE ACCOUNT_ACTIVATIONS (
    activation_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    user_id UNIQUEIDENTIFIER NOT NULL,
    created_by UNIQUEIDENTIFIER NULL,
    approved_by UNIQUEIDENTIFIER NULL,
    token VARCHAR(255),
    status VARCHAR(50),
    created_at DATETIME2,
    approved_at DATETIME2,
    activated_at DATETIME2,
    expires_at DATETIME2
);

CREATE TABLE PASSWORD_RESET_TOKENS (
    token_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    user_id UNIQUEIDENTIFIER NOT NULL,
    token VARCHAR(255) NOT NULL,
    expires_at DATETIME2 NOT NULL,
    used BIT NOT NULL,
    created_at DATETIME2 NOT NULL
);

CREATE TABLE SUBSCRIPTIONS (
    subscription_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    user_id UNIQUEIDENTIFIER NOT NULL,
    plan_name VARCHAR(50),
    start_date DATETIME2,
    end_date DATETIME2,
    active BIT NOT NULL
);

CREATE TABLE B2B_CLIENTS (
    client_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    company_name VARCHAR(255),
    legal_name VARCHAR(255),
    commercial_contact VARCHAR(255),
    technical_contact VARCHAR(255),
    billing_address VARCHAR(500),
    plan_name VARCHAR(50),
    monthly_quota INT,
    sla VARCHAR(100),
    active BIT NOT NULL,
    created_at DATETIME2 NOT NULL
);

CREATE TABLE API_KEYS (
    api_key_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    client_id UNIQUEIDENTIFIER NOT NULL,
    label VARCHAR(100),
    api_key_hash VARCHAR(255),
    status VARCHAR(50),
    created_at DATETIME2,
    expires_at DATETIME2
);

CREATE TABLE CARDS (
    card_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    created_by UNIQUEIDENTIFIER NOT NULL,
    current_version_id UNIQUEIDENTIFIER NULL,
    active BIT NOT NULL,
    created_at DATETIME2 NOT NULL
);

CREATE TABLE CARD_VERSIONS (
    version_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    card_id UNIQUEIDENTIFIER NOT NULL,
    set_name VARCHAR(255),
    card_number VARCHAR(50),
    edition VARCHAR(50),
    language VARCHAR(50),
    finish_type VARCHAR(50),
    name VARCHAR(255),
    rarity VARCHAR(100),
    pokemon_type VARCHAR(100),
    hp INT,
    illustrator VARCHAR(255),
    release_year INT,
    psa_grade DECIMAL(3,1),
    psa_certificate VARCHAR(100),
    created_by UNIQUEIDENTIFIER,
    created_at DATETIME2 NOT NULL
);

CREATE TABLE CARD_IMAGES (
    image_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    version_id UNIQUEIDENTIFIER NOT NULL,
    image_type VARCHAR(50),
    image_url VARCHAR(1000)
);

CREATE TABLE IMPORT_BATCHES (
    batch_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    created_by UNIQUEIDENTIFIER NOT NULL,
    file_name VARCHAR(255),
    file_hash VARCHAR(255),
    total_rows INT,
    valid_rows INT,
    invalid_rows INT,
    status VARCHAR(50),
    created_at DATETIME2 NOT NULL
);

CREATE TABLE IMPORT_BATCH_ITEMS (
    item_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    batch_id UNIQUEIDENTIFIER NOT NULL,
    row_number INT,
    status VARCHAR(50),
    error_message VARCHAR(MAX),
    card_id UNIQUEIDENTIFIER NULL
);

CREATE TABLE AUDIT_LOGS (
    audit_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    user_id UNIQUEIDENTIFIER NOT NULL,
    action_type VARCHAR(100),
    entity_name VARCHAR(100),
    entity_id UNIQUEIDENTIFIER,
    old_value VARCHAR(MAX),
    new_value VARCHAR(MAX),
    reason VARCHAR(255),
    ip_address VARCHAR(100),
    timestamp DATETIME2 NOT NULL
);
GO
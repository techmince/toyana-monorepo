-- Database: toyana_ordering
CREATE DATABASE toyana_ordering;
\c toyana_ordering;

-- Note: When using Wolverine + Marten, the 'Bookings' and Saga state are managed 
-- automatically as JSONB documents (mt_doc_booking).
-- However, for analytics or external reporting, a relational structure is often desired.
-- Below is the schema representing the Booking state.

CREATE TABLE Bookings (
    Id UUID PRIMARY KEY,
    VendorId UUID NOT NULL,
    UserId UUID NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status VARCHAR(50) NOT NULL, -- PendingVendorApproval, AwaitingPayment, Confirmed, Completed
    EventDate TIMestamptz NOT NULL,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

-- Transactional Outbox Table (Wolverine standard structure reference)
CREATE TABLE IF NOT EXISTS wolverine_outbox (
    id UUID PRIMARY KEY,
    owner_id INTEGER NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT (now()),
    data BYTEA NOT NULL
    -- other columns managed by Wolverine
);

-- Saga State Table (if storing relationally)
CREATE TABLE BookingSagas (
    Id UUID PRIMARY KEY,
    CorrelationId UUID,
    CurrentState VARCHAR(255), -- Serialized state or status column
    SagaData JSONB, -- The full saga state blob
    IsCompleted BOOLEAN DEFAULT FALSE
);

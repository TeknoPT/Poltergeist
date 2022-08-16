﻿using Poltergeist.PhantasmaLegacy.Cryptography;
using Poltergeist.PhantasmaLegacy.Numerics;

namespace Poltergeist.PhantasmaLegacy.Domain
{
    public enum EventKind
    {
        Unknown = 0,
        ChainCreate = 1,
        TokenCreate = 2,
        TokenSend = 3,
        TokenReceive = 4,
        TokenMint = 5,
        TokenBurn = 6,
        TokenStake = 7,
        TokenClaim = 8,
        AddressRegister = 9,
        AddressLink = 10,
        AddressUnlink = 11,
        OrganizationCreate = 12,
        OrganizationAdd = 13,
        OrganizationRemove = 14,
        GasEscrow = 15,
        GasPayment = 16,
        AddressUnregister = 17,
        OrderCreated = 18,
        OrderCancelled = 19,
        OrderFilled = 20,
        OrderClosed = 21,
        FeedCreate = 22,
        FeedUpdate = 23,
        FileCreate = 24,
        FileDelete = 25,
        ValidatorPropose = 26,
        ValidatorElect = 27,
        ValidatorRemove = 28,
        ValidatorSwitch = 29,
        PackedNFT = 30,
        ValueCreate = 31,
        ValueUpdate = 32,
        PollCreated = 33,
        PollClosed = 34,
        PollVote = 35,
        ChannelCreate = 36,
        ChannelRefill = 37,
        ChannelSettle = 38,
        LeaderboardCreate = 39,
        LeaderboardInsert = 40,
        LeaderboardReset = 41,
        PlatformCreate = 42,
        ChainSwap = 43,
        ContractRegister = 44,
        ContractDeploy = 45,
        AddressMigration = 46,
        ContractUpgrade = 47,
        Log = 48,
        Inflation = 49,
        OwnerAdded = 50,
        OwnerRemoved = 51,
        DomainCreate = 52,
        DomainDelete = 53,
        TaskStart = 54,
        TaskStop = 55,
        CrownRewards = 56,
        Infusion = 57,
        Custom = 64,
    }

    public struct TokenEventData
    {
        public readonly string Symbol;
        public readonly BigInteger Value;
        public readonly string ChainName;

        public TokenEventData(string symbol, BigInteger value, string chainName)
        {
            this.Symbol = symbol;
            this.Value = value;
            this.ChainName = chainName;
        }
    }

    public struct GasEventData
    {
        public readonly Address address;
        public readonly BigInteger price;
        public readonly BigInteger amount;

        public GasEventData(Address address, BigInteger price, BigInteger amount)
        {
            this.address = address;
            this.price = price;
            this.amount = amount;
        }
    }

    public struct Event
    {
        public EventKind Kind { get; private set; }
        public Address Address { get; private set; }
        public string Contract { get; private set; }
        public byte[] Data { get; private set; }

        public Event(EventKind kind, Address address, string contract, byte[] data = null)
        {
            this.Kind = kind;
            this.Address = address;
            this.Contract = contract;
            this.Data = data;
        }
    }
}

export interface Consigner {
    id: number;
    name: string;
    email?: string;
    phone?: string;
    paymentDetails?: string;
    unpaidBalance: number;
    totalPaidOut: number;
    commissionRate: number;
    notes?: string;
    isActive: boolean;
}

export interface ConsignerPayout {
    id: number;
    consignerId: number;
    amount: number;
    payoutDate: string;
    paymentMethod?: string;
    transactionReference?: string;
    notes?: string;
}

export interface UpsertConsignerRequest {
    id?: number;
    name: string;
    email?: string;
    phone?: string;
    paymentDetails?: string;
    commissionRate: number;
    notes?: string;
    isActive: boolean;
}

export interface RecordPayoutRequest {
    amount: number;
    paymentMethod?: string;
    transactionReference?: string;
    notes?: string;
}

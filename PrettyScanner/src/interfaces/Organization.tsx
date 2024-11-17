export interface Organization {
    id: number | undefined | null;
    name: string;
    slug: string;
    logoUrl?: string;
    createdAt: Date;
    isActive: boolean;
    subscriptionTier?: string;
}

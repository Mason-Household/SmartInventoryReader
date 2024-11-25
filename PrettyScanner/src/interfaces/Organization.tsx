export interface Organization {
	id: number | null | undefined;
	name: string;
	slug: string;
	createdAt: Date;
	isActive: boolean;
	subscriptionTier: string;
	firebaseTenantId?: string;
}
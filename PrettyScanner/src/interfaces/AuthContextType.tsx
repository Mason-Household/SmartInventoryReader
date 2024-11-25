import { User } from "firebase/auth";
import { Organization } from "./Organization";

export interface AuthContextType {
	user: User | null;
	organization: Organization | null;
	organizations: Organization[];
	setCurrentOrganization: (org: Organization) => void;
	loginWithGoogle: () => Promise<void>;
	loginWithEmail: (email: string, password: string) => Promise<void>;
	registerWithEmail: (email: string, password: string, organizationName: string) => Promise<void>;
	loginWithHuggingFace: (token: string, org: Organization) => Promise<void>;
	logout: () => Promise<void>;
	isAuthenticated: boolean;
	isLoading: boolean;
}
export interface IToast {
    id?: number;
    message: string;
    title?: string;
    type?: "error" | "info" | "info-square" | "success" | "warning" | "warning-alt";
    dismissible?: boolean;
    timeout?: number;
    dismissAt?: number;
    hovered?: boolean;
}
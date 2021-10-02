export interface IServiceStatus {
    online: boolean;
    lastDisconnect?: Date;
    responseTime: number;
    message?: string;
}
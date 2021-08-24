export interface TemplateSettings {
    name: string;
    viewPermission: TemplateViewPermission
}

export enum TemplateViewPermission {
    Global,
    Guild,
    Self
}

export function TemplateViewPermissionOptions() : Array<string> {
    let keys = Object.keys(TemplateViewPermission);
    return keys.slice(keys.length / 2);
}

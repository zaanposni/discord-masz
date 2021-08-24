export enum PunishmentType {
    None,
    Mute,
    Kick,
    Ban
}

export function PunishmentTypeOptions() : Array<string> {
    let keys = Object.keys(PunishmentType);
    return keys.slice(keys.length / 2).map(x => { if(x === "None") { return "Warn" } else { return x } });
}

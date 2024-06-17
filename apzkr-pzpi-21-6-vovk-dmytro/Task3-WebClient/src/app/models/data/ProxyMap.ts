export class ProxyMap<K, V> extends Map<K, V> {
    protected _events: MapEventRecordArray = {};
    private _maxListeners: number = 0;

    override set(key: K, value: V): this {
        super.set(key, value);
        this.emit("set", key, value, this);
        return this;
    }

    override delete(key: K): boolean {
        const result = super.delete(key);
        this.emit("delete", key, this);
        return result;
    }

    override clear(): void {
        super.clear();
        this.emit("clear", this);
    }

    constructor(entries?: readonly (readonly [K, V])[] | null) {
        super(entries);
        this._events = {};
    }

    addListener(eventName: MapEvent, listener: MapEventCallback<K, V>): this {
        return this.on(eventName, listener);
    }

    on(eventName: MapEvent, listener: MapEventCallback<K, V>): this {
        if (this._events[eventName]?.length >= this._maxListeners && this._maxListeners > 0) {
            throw new Error("Max listeners reached");
        }
        this._events[eventName] = this._events[eventName] || [];
        this._events[eventName].push({
            fun: listener,
            once: false
        });
        return this;
    }

    off(eventName: MapEvent, listener: MapEventCallback<K, V>): this {
        if (this._events[eventName]) {
            const index = this._events[eventName].findIndex((e) => e.fun === listener);
            if (index >= 0) {
                this._events[eventName].splice(index, 1);
            }
        }
        return this;
    }
    removeAllListeners(event?: MapEvent | undefined): this {
        if (event) {
            this._events[event] = [];
        } else {
            this._events = {};
        }
        return this;
    }
    setMaxListeners(n: number): this {
        this._maxListeners = n;
        return this;
    }
    getMaxListeners(): number {
        return this._maxListeners;
    }
    listeners(eventName: MapEvent): Function[] {
        return [...this._events[eventName].map((e) => e.fun)];
    }
    rawListeners(eventName: MapEvent): Function[] {
        return this._events[eventName].map((e) => e.fun);
    }
    emit(eventName: MapEvent, ...args: any[]): boolean {
        if (this._events != undefined && this._events[eventName]) {
            this._events[eventName].forEach((e) => {
                e.fun(...args);
                if (e.once) {
                    this.off(eventName, e.fun);
                }
            });
        }
        return true;
    }
    listenerCount(eventName: MapEvent): number {
        if (this._events[eventName]) {
            return this._events[eventName].length;
        }
        return 0;
    }
    eventNames(): (MapEvent)[] {
        return Object.keys(this._events) as (MapEvent)[];
    }
}

export type MapEvent = "set" | "delete" | "clear";
export type MapEventCallback<K, V> = MapEventCallbackSet<K, V> | MapEventCallbackDelete<K, V> | MapEventCallbackClear<K, V>;
export type MapEventCallbackSet<K, V> = (key: K, value: V, map: ProxyMap<K, V>) => void;
export type MapEventCallbackDelete<K, T> = (key: K, map: ProxyMap<K, T>) => void;
export type MapEventCallbackClear<K, T> = (map: ProxyMap<K, T>) => void;
type MapEventRecordArray = {
    [key: string]: {
        fun: (...args: any[]) => void | Function,
        once: boolean
    }[]
}
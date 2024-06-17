import { EventEmitter } from "events";

export class ProxyArray<T> extends Array<T> {
    protected _events: ArrayEventRecordArray = {};
    private _maxListeners: number = 0;

    override push(...items: T[]): number {
        items.forEach((item) => {
            super.push(item);
            this.emit("push", item, this);
        });
        return items.length;
    }

    override pop(): T | undefined {
        const result = super.pop();
        this.emit("pop", result, this);
        return result;
    }

    override shift(): T | undefined {
        const result = super.shift();
        this.emit("shift", result, this);
        return result;
    }

    override unshift(...items: T[]): number {
        const result = super.unshift(...items);
        this.emit("unshift", items, this);
        return result;
    }

    override splice(start: number, deleteCount: number, ...items: T[]): T[] {
        const result = super.splice(start, deleteCount, ...items);
        this.emit("splice", result, this);
        return result;
    }

    override sort(compareFn?: (a: T, b: T) => number): this {
        super.sort(compareFn);
        this.emit("sort", this);
        return this;
    }

    override reverse(): this {
        super.reverse();
        this.emit("reverse", this);
        return this;
    }

    addListener(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        return this.on(eventName, listener);
    }

    on(eventName: string, listener: ArrayEventCallback<T>): this {
        this._events[eventName] = this._events[eventName] || [];
        if (this._events[eventName].length >= this._maxListeners && this._maxListeners > 0) {
            throw new Error("Max listeners reached");
        }
        this._events[eventName].push({
            fun: listener,
            once: false
        });
        return this;
    }
    once(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        this._events[eventName] = this._events[eventName] || [];
        this._events[eventName].push({
            fun: listener,
            once: true
        });
        return this;
    }
    removeListener(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        return this.off(eventName, listener);
    }
    off(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        if (this._events[eventName]) {
            const index = this._events[eventName].findIndex((e) => e.fun === listener);
            if (index >= 0) {
                this._events[eventName].splice(index, 1);
            }
        }
        return this;
    }
    removeAllListeners(event?: ArrayEvent | undefined): this {
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
    listeners(eventName: ArrayEvent): Function[] {
        return [...this._events[eventName].map((e) => e.fun)];
    }
    rawListeners(eventName: ArrayEvent): Function[] {
        return this._events[eventName].map((e) => e.fun);
    }
    emit(eventName: ArrayEvent, ...args: any[]): boolean {
        if (this._events[eventName]) {
            this._events[eventName].forEach((e) => {
                e.fun(...args);
                if (e.once) {
                    this.off(eventName, e.fun);
                }
            });
        }
        return true;
    }
    listenerCount(eventName: ArrayEvent): number {
        if (this._events[eventName]) {
            return this._events[eventName].length;
        }
        return 0;
    }
    prependListener(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        this._events[eventName] = this._events[eventName] || [];
        this._events[eventName].unshift({
            fun: listener,
            once: false
        });
        return this;
    }
    prependOnceListener(eventName: ArrayEvent, listener: ArrayEventCallback<T>): this {
        this._events[eventName] = this._events[eventName] || [];
        this._events[eventName].unshift({
            fun: listener,
            once: true
        });
        return this;
    }
    eventNames(): (ArrayEvent)[] {
        return Object.keys(this._events) as (ArrayEvent)[];
    }
}

export type ArrayEvent = "push" | "pop" | "shift" | "unshift" | "splice" | "sort" | "reverse";
export type ArrayEventCallback<T> = (entity: T, array: T[]) => void;
type ArrayEventRecordArray = {
    [key: string]: {
        fun: (...args: any[]) => void | Function,
        once: boolean
    }[]
}
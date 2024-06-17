export class PageResponse<T> {
    public total: number = 0;
    public page: number = 1;
    public limit: number = 10;
    public items: T[] = [];
}
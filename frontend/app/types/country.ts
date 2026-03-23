import type { NestedGenre } from "./nestedgenre"

export interface Country {
    id: string
    name: string
    description: string
    genres: NestedGenre[]
}
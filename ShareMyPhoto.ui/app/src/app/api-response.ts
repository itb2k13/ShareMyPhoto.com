export class input {
    originalUrl: string;
}
export class output {
    share: string;
    sizeInBytes: number;
}

export class ApiResponse {
    input: input;
    output: output;
    errorMessage: string;
    success: boolean;
}

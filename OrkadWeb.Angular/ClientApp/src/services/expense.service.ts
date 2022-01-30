import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { AddExpenseCommand } from "src/shared/models/expenses/AddExpenseCommand";
import { AddExpenseResult } from "src/shared/models/expenses/AddExpenseResult";

@Injectable({ providedIn: "root" })
export class ExpenseService {
  constructor(private httpClient: HttpClient) {}

  add(command: AddExpenseCommand): Observable<AddExpenseResult> {
    return this.httpClient.post<AddExpenseResult>("api/expense/add", command);
  }
}

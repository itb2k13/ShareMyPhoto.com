import { Inject, Injectable } from '@angular/core';
import { LOCAL_STORAGE, StorageService } from 'ngx-webstorage-service';

@Injectable({
  providedIn: 'root'
})
export class LocalStoreService {

  STORAGE_KEY = 'sh.are.photos';

  constructor(@Inject(LOCAL_STORAGE) private storage: StorageService) { }

  public saveLocal(url: string): void {
    if (url) {
      var currentTodoList = this.storage.get(this.STORAGE_KEY) || [];
      currentTodoList.push(url);
      this.storage.set(this.STORAGE_KEY, currentTodoList);
    }
  }

  public getLocal() {
    return this.storage.get(this.STORAGE_KEY) || [];
  }

}

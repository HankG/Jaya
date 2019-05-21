import { Injectable } from '@angular/core';
import { SuperService } from '@shared/super.service';
import { IpcService } from '@services/ipc.service';
import { MessageType, ConfigModel, Helpers } from '@common/index';

@Injectable()
export class ConfigService extends SuperService {
    
    constructor(private readonly _ipc: IpcService) {
        super();
    }

    protected Dispose(): void {
        this._ipc.Receive.unsubscribe();
    }

    async GetOrSetConfiguration<T>(key: string, value: T): Promise<T> {
        let config = new ConfigModel(key, value)
        const configJson = Helpers.Serialize<ConfigModel>(config);
        const response = await this._ipc.SendAsync(MessageType.GetSetConfig, configJson);
        config = Helpers.Deserialize<ConfigModel>(response.DataJson, ConfigModel);
        return null;
    }

    DeleteConfiguration(key: string): void {
        this._ipc.Send(MessageType.DeleteConfig, key);
    }

}
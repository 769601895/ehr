import { PublicConfigClass } from '@/packages/public'
import { VChartPieConfig } from './index'
import { CreateComponentType } from '@/packages/index.d'
import { vChartOptionPrefixHandle } from '@/packages/public/vChart'
import data from './data.json'
import cloneDeep from 'lodash/cloneDeep'
import { IPieOption } from '../../index.d'

export const includes = ['legends', 'tooltip']
export const option: IPieOption & { dataset?: any } = {
  // 图表配置
  type: 'pie',
  dataset: data,
  categoryField: 'year',
  valueField: 'value',
  seriesField: 'year',
  // 业务配置（后续会被转换为图表spec)
  category: VChartPieConfig.category,
}

export default class Config extends PublicConfigClass implements CreateComponentType {
  public key = VChartPieConfig.key
  public chartConfig = cloneDeep(VChartPieConfig)
  // 图表配置项
  public option = vChartOptionPrefixHandle(option, includes)
}

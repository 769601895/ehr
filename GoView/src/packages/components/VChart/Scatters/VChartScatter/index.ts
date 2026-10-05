import { ConfigType, PackagesCategoryEnum, ChartFrameEnum } from '@/packages/index.d'
import { ChatCategoryEnum, ChatCategoryEnumName } from '../../index.d'

export const VChartScatterConfig: ConfigType = {
  key: 'VChartScatter',
  chartKey: 'VVChartScatter',
  conKey: 'VCVChartScatter',
  title: 'VChart散点图',
  category: ChatCategoryEnum.SCATTER,
  categoryName: ChatCategoryEnumName.SCATTER,
  package: PackagesCategoryEnum.VCHART,
  chartFrame: ChartFrameEnum.VCHART,
  image: 'vchart_scatter.png'
}

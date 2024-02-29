namespace Components


open Model
open Feliz
open Feliz.DaisyUI
open Feliz.Recharts
open Fable.Core.JsInterop

type private Data = 
    {
        name: string; 
        value: int
    }
    static member create(name, value) = {name = name; value = value}

open System


module Extension =
    module Interop =
        let inline mkSectorAttr (key: string) (value: obj) : ISectorProperty = unbox (key, value)

    type pie with
        static member inline paddingAngle (angle:int) = Interop.mkPieAttr "paddingAngle" angle
        static member inline activeShape (props: IPieLabelProperties -> Feliz.ReactElement) : IPieProperty = Interop.mkPieAttr "activeShape" props
        static member inline onAnimationStart (func: unit -> unit) : IPieProperty = Interop.mkPieAttr "onAnimationStart" func
        static member inline onAnimationEnd (func: unit -> unit) : IPieProperty = Interop.mkPieAttr "onAnimationEnd" func

    type sector =
        static member inline cx (value:int) = Interop.mkSectorAttr "cx" value
        static member inline cx (value:float) = Interop.mkSectorAttr "cx" value
        static member inline cy (value:int) = Interop.mkSectorAttr "cy" value
        static member inline cy (value:float) = Interop.mkSectorAttr "cy" value
        static member inline innerRadius (value:float) = Interop.mkSectorAttr "innerRadius" value
        static member inline innerRadius (value:int) = Interop.mkSectorAttr "innerRadius" value
        static member inline outerRadius (value:float) = Interop.mkSectorAttr "outerRadius" value
        static member inline outerRadius (value:int) = Interop.mkSectorAttr "outerRadius" value
        static member inline startAngle (value:float) = Interop.mkSectorAttr "startAngle" value
        static member inline startAngle (value:int) = Interop.mkSectorAttr "startAngle" value
        static member inline endAngle (value:int) = Interop.mkSectorAttr "endAngle" value
        static member inline endAngle (value:float) = Interop.mkSectorAttr "endAngle" value
        static member inline fill (value:string) = Interop.mkSectorAttr "fill" value
        

open Extension

module private Helper =
    let RADIAN = Math.PI / 180.;

    let renderActiveShape = fun (getUseRyRole: string -> string) (props: IPieLabelProperties) -> 
        let midAngle, cx, cy, outerRadius, innerRadius, fill, (startAngle: int), (endAngle: int) = props.midAngle, props.cx, props.cy, props.outerRadius, props.innerRadius, props?fill, props?startAngle, props?endAngle
        let sin = Math.Sin(-RADIAN * midAngle)
        let cos = Math.Cos(-RADIAN * midAngle)
        let sx = cx + (outerRadius + 10.) * cos
        let sy = cy + (outerRadius + 10.) * sin
        let mx = cx + (outerRadius + 30.) * cos
        let my = cy + (outerRadius + 30.) * sin
        let ex = mx + (if cos >= 0. then 1. else -1.) * 22.;
        let ey = my;
        let textAnchor = if cos >= 0 then svg.textAnchor.startOfText else svg.textAnchor.endOfText;
        Svg.g [
            Svg.text [
                svg.x cx; svg.y cy; svg.textAnchor.middle; svg.fill fill
            ]
            Recharts.sector [
                sector.cx cx
                sector.cy cy
                sector.innerRadius innerRadius
                sector.outerRadius outerRadius
                sector.startAngle startAngle
                sector.endAngle endAngle
                sector.fill fill
            ]
            Recharts.sector [
                sector.cx cx
                sector.cy cy
                sector.innerRadius (innerRadius + 6.)
                sector.outerRadius (outerRadius + 10.)
                sector.startAngle startAngle
                sector.endAngle endAngle
                sector.fill fill
            ]
            //<path d={`M${sx},${sy}L${mx},${my}L${ex},${ey}`} stroke={fill} fill="none" />
            Svg.path [
                svg.d (sprintf "M%i,%iL%i,%iL%i,%i" (int sx) (int sy) (int mx) (int my) (int ex) (int ey))
                svg.stroke fill
                svg.fill "none"
            ]
            //<circle cx={ex} cy={ey} r={2} fill={fill} stroke="none" />
            Svg.circle [
               svg.cx ex
               svg.cy ey
               svg.r 2
               svg.fill fill
               svg.stroke "none" 
            ]
            //<text x={ex + (cos >= 0 ? 1 : -1) * 12} y={ey} textAnchor={textAnchor} fill="#333">{`PV ${value}`}</text>
            Svg.text [
              svg.x (ex + (if cos >= 0. then 1. else -1.) * 12.)
              svg.y ey
              textAnchor
              svg.fill "#333"
              svg.text (getUseRyRole props?name)
            ]
        ]


type Wheel =

    [<ReactComponent>]
    static member Main(roles: Role list, users: string list) =
        let showUsers, setShowUsers = React.useState(false)
        let data0 = 
            let n = roles.Length
            let v = 100 / n
            let c = fun (n: Role) -> Data.create(string n,v)
            List.map c roles
        let activeIndices = [0 .. users.Length-1]
        let getUseRyRole = fun (roleStr: string) ->
          let role = Role.fromString roleStr
          let index = List.tryFindIndex (fun x -> x = role) roles
          match index with
          | Some i -> 
            match List.tryItem i users with
            | Some user -> user
            | None -> ""
          | None -> ""
        Recharts.responsiveContainer [
            responsiveContainer.chart <|
                Recharts.pieChart [
                    pieChart.height 500
                    pieChart.width 500
                    // pieChart.data data0
                    pieChart.children [
                        Recharts.pie [
                            pie.data data0
                            pie.fill "#8884d8"
                            pie.dataKey (fun (data: Data) -> data.value)
                            pie.labelLine false
                            pie.outerRadius.percentage 80.
                            pie.innerRadius.percentage 30.
                            pie.paddingAngle 5
                            if showUsers then pie.activeIndex activeIndices
                            pie.onAnimationEnd (fun _ -> setShowUsers true; log "End")
                            pie.activeShape (Helper.renderActiveShape getUseRyRole)
                            pie.children [
                                Recharts.labelList [
                                    labelList.dataKey (fun (data:Data) -> data.name)
                                    labelList.position.right
                                ]
                                for i in 0 .. data0.Length-1 do
                                    let role = List.item i roles
                                    let color = role.ToColor()
                                    Recharts.cell [
                                        cell.key (string i)
                                        cell.fill (color)
                                    ]
                            ]
                        ]
                        Recharts.tooltip []
                    ]
                ]
        ]
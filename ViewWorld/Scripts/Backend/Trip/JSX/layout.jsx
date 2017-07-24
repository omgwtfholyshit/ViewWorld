
var menu = [["账户管理", "anticon-user", [["我的账户", "href"]]], ["旅游管理", "anticon-user", [["旅游产品分类", "href"], ["行程管理", "href"], ["景点管理", "href"], ["出发地管理", "href"]]],
    ["订单管理", "anticon-user", [["旅行团订单", "href"], ["机票订单-二期", "href"]]], ["会员管理", "anticon-user", [["会员列表", "href"], ["信用卡信息", "href"], ["优惠券管理", "href"]]]
    , ["供应商管理", "anticon-user", [["供应商管理", "href"]]], ["系统设置", "anticon-user", [["通用设置", "href"]]]
]
var Layout = React.createClass({
    render: function () {
        return(
           <div>
               <Navi menu={menu}></Navi>
               <Header></Header>
           </div>
            )
    }
})
var Navi = React.createClass({
    render: function () {
        var menuItem = this.props.menu.map(function (item,index) {
            var subMenu = item[2].map(function (it, i) {
                return(
                    <li key={i} className="ant-menu-item" role="menuitem" aria-selected="false" data-href={it[1]} >{it[0]}</li>
                    )
            })
            return (
                <li className="ant-menu-submenu-inline ant-menu-submenu" key={index}>
                    <div className="ant-menu-submenu-title" aria-open="false" aria-owns="{index}" aria-haspopup="true" >
                        <span><i className="anticon anticon-laptop"></i>{item[0]}</span>
                    </div>
                    <ul className="ant-menu ant-menu-inline  ant-menu-sub" role="menu" aria-activedescendant="">
                        {subMenu}
                    </ul>
                 </li>
                )
        });
        
        return(
            <div className="ant-col-4" id="Navi">
                <div className="ant-layout-logo"></div>
                <ul className="ant-menu ant-menu-inline  ant-menu-dark ant-menu-root" role="menu" aria-activedescendant="" tabIndex="0">
                    {menuItem}
                </ul>
            </div>
            )
    }
})
var Header = React.createClass({
    render: function () {
        return(
           <div id="Header" className="ant-col-20">
               Header
           </div>
            )
    }
})
ReactDOM.render(<Layout  />, document.getElementById('react-content'))

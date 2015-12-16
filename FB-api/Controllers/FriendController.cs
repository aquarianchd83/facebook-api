using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FB_api.Models;
using Facebook;
using Newtonsoft.Json;

namespace FB_api.Controllers
{
    public class FriendController : BaseController
    {
        // GET: /Friends/
        public ActionResult Index()
        {
            FacebookFriendsModel friends = new FacebookFriendsModel();

            var client = new FacebookClient(Helpers.StaticValue.Profile.AccessToken);

            dynamic fbresult = client.Get("me/friends");

            var data = fbresult["data"].ToString();

            friends.friendsListing = JsonConvert.DeserializeObject<List<FacebookFriend>>(data);

            return View("Index", friends);
        }
    }
}

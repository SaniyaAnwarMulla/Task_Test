using AutoMapper;
using Task.DBContext;
using Task.Model;

namespace Task.Services
{
    public interface IUser
    {
        ResponseModel GetAll();
        ResponseModel Save(User user);
        ResponseModel Delete(int id);
        ResponseModel Get(int id);
        string ValidateLogin(string userName, string password);
    }
    public class UserRepo : IUser
    {
        Mapper _mapper = null;
        TaskContext db;
        public UserRepo(TaskContext db)
        {
            this.db = db;

            MapperConfiguration config= new MapperConfiguration(cfg=>cfg.CreateMap<User,TblUser>().ReverseMap());
            _mapper = new Mapper(config);

        }

        public ResponseModel Delete(int id)
        {
            ResponseModel res=new ResponseModel();
            try
            {
                var o = db.TblUsers.Find(id);
                if (o != null)
                {
                    db.TblUsers.Remove(o);
                    db.SaveChanges();

                }
                res.Code = 0;
                res.Message = "Delete";
            }
            catch (Exception er)
            {
                res.Code = -1;
                res.Message = er.Message;
            }
            return res;
        }

        public ResponseModel Get(int id)
        {
            ResponseModel res = new ResponseModel();

            if (id ==0)
            {
                res.Code = 2;
                res.Message = "Invalid UserId";
                return res;
            }
            try
            {

                var o = db.TblUsers.Find(id);
                User eo = _mapper.Map<User>(o);
                res.Code = 0;
                res.Message = eo;
               
            }
            catch (Exception er)
            {
                res.Code = -1;
                res.Message = er.Message;
                
            }
            return res;
        }

        public ResponseModel GetAll()
        {
            ResponseModel res = new ResponseModel();
            try
            {
                var oList = db.TblUsers.ToList();
                List<User> ulist = _mapper.Map<List<User>>(oList);

                res.Code = 0;
                res.Message = ulist;
                
            }
            catch (Exception er)
            {
                res.Code = 0;
                res.Message = er.Message;

            }
            return res;
        }
        public ResponseModel Save(User user)
        {
            ResponseModel res = new ResponseModel();
            try
            {
                TblUser o = _mapper.Map<TblUser>(user);
                db.Entry(o).State=user.Id>0? Microsoft.EntityFrameworkCore.EntityState.Modified:Microsoft.EntityFrameworkCore.EntityState.Added; 
                db.SaveChanges();

                res.Code = 0;
                res.Message = "Record Created";
            }
            catch (Exception er)
            {
                res.Code = 0;
                res.Message = er.Message;
            }

            return res;
        }

        public string ValidateLogin(string userName, string password)
        {
            var obj=db.TblUsers.Where(w=>w.Username.Trim().ToLower()==userName.Trim().ToLower() && w.Password.Trim().ToLower()==password.Trim().ToLower()).FirstOrDefault();
            if (obj!=null) 
            {
                return obj.Username;
            }
            return null;
        }
    }
}

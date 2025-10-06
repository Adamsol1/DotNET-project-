

/*
Here the Idea is to create a Base Controller that owns functionalities
that is common and used by all the controllers.

I will create som costum controller functions, that essentially
removes the need to repeat the same code in each controller
such as CreateResponse  which trows exceptions for bad requests,
Once Eirik has created the loggers, these can be used here. 
allowing us to have easy controller.

if you think we need more stuff, we can add them here.
*/

[ApiController]
public class BaseCustomController : ControllerBase
{

    // this doesnt need a constructor, as we will not be using any dependencies
    // for now.

    /*
    ValidateAndExcecute 
    */

}



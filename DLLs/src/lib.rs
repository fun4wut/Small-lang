
#[no_mangle]
pub extern fn print_int(x: i64) {
    println!("{}", x);
}

#[no_mangle]
pub extern fn print_float(x: f64) {
    println!("{}", x);
}

#[no_mangle]
pub extern fn print_bool(x: bool) {
    println!("{}", x);
}


#[cfg(test)]
mod tests {
    #[test]
    fn it_works() {
        assert_eq!(2 + 2, 4);
    }
}
